using AutoMapper;
using Comic.API.Code.Dtos;
using Comic.API.Code.Helpers;
using Comic.API.Code.Interfaces;
using Comic.API.Data;
using Comic.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Comic.API.Code.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ComicDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public IdentityService(UserManager<AppUser> userManager,
                           SignInManager<AppUser> signInManager,
                           ComicDbContext db,
                           ICurrentUser currentUser,
                           JwtSettings jwtSettings,
                           IMapper mapper,
                           RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _currentUser = currentUser;
        _jwtSettings = jwtSettings;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var appUser = await _userManager.FindByEmailAsync(_currentUser.Email);
        return await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = _mapper.Map<AppUser>(userForRegistration);
        user.UserName = userForRegistration.Email;

        var result = await _userManager.CreateAsync(user, userForRegistration.Password);

        if (result.Succeeded)
        {
            await CreateRolesIfNotExist(userForRegistration.Roles);
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
        }

        return result;
    }

    private async Task CreateRolesIfNotExist(IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        var user = await _userManager.FindByEmailAsync(userForAuth.Email);

        var result = (user != null && await _userManager.CheckPasswordAsync(user, userForAuth.Password));

        return result;
    }

    public async Task<TokenDto> CreateToken(string email)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new ArgumentException($"User with email '{email}' not found.");
        }

        await _userManager.RemoveAuthenticationTokenAsync(user, "Auth", "RefreshToken");

        var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "Auth", "RefreshToken");

        await _userManager.SetAuthenticationTokenAsync(user, "Auth", "RefreshToken", newRefreshToken);

        var accessToken = GenerateAccessToken(user);

        return new TokenDto(accessToken, newRefreshToken);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<TokenDto> RefreshToken(string refreshToken)
    {
        // Validate the refreshToken parameter
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        }

        // Check if the user's email is available
        if (string.IsNullOrEmpty(_currentUser.Email))
        {
            throw new InvalidOperationException("User email is not available.");
        }

        var user = await _userManager.FindByEmailAsync(_currentUser.Email);

        // Verify the refresh token
        var isValid = await _userManager.VerifyUserTokenAsync(user, "Auth", "RefreshToken", refreshToken);

        if (!isValid)
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        // Generate a new access token
        return await CreateToken(_currentUser.Email);
    }

    private string GenerateAccessToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.UserId, user.Id.ToString()),
            new Claim(ClaimTypes.FirstName, user.FirstName),
            new Claim(ClaimTypes.LastName, user.LastName),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.ValidIssuer,
            Audience = _jwtSettings.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}
