using Comic.API.Code.Extensions;
using Comic.API.Code.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Comic.API.Code.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated { get { return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated; } }
    public bool IsAdmin { get { return _httpContextAccessor.HttpContext.User.IsInRole("Admin"); } }
    public bool IsUser { get { return _httpContextAccessor.HttpContext.User.IsInRole("User"); } }

    public int? Id { get { return GetClaim(ClaimTypes.UserId).GetId(); } }
    public string FirstName { get { return GetClaim(ClaimTypes.FirstName); } } 
    public string LastName { get { return GetClaim(ClaimTypes.LastName); } }
    public string Email { get { return GetClaim(ClaimTypes.Email); } }

    private string GetClaim(string name)
    {
        var token = GetAccessToken();

        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Retrieve the claim from the decoded token
            return jsonToken?.Claims.FirstOrDefault(c => c.Type == name)?.Value;
        }

        return null;
    }

    private string GetAccessToken()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
        {
            return authorizationHeader.Substring("Bearer ".Length);
        }

        return null;
    }

}