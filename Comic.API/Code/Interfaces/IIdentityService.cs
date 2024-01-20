using Comic.API.Code.Dtos;
using Comic.API.Domain;
using Microsoft.AspNetCore.Identity;

namespace Comic.API.Code.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<TokenDto> CreateToken(string email);
        Task SignOutAsync();
        Task<IdentityResult> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<TokenDto> RefreshToken(string refreshToken);
    }
}
