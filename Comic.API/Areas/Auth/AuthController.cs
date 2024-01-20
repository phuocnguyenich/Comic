using Comic.API.Areas.ActionFilters;
using Comic.API.Code.Dtos;
using Comic.API.Code.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var result = await _identityService.RegisterUser(userForRegistration);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _identityService.ValidateUser(user))
                return Unauthorized();

            var tokenDto = await _identityService.CreateToken(user.Email);

            return Ok(tokenDto);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var tokenDtoToReturn = await _identityService.RefreshToken(refreshToken);

            return Ok(tokenDtoToReturn);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _identityService.SignOutAsync();

            return Ok(new { Message = "User signed out successfully." });
        }

        [HttpPost("change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _identityService.ChangePasswordAsync(changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Password changed successfully." });
            }

            return BadRequest(new { Message = "Failed to change password.", Errors = result.Errors });
        }
    }
}
