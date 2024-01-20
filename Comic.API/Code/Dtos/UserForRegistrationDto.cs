using System.ComponentModel.DataAnnotations;

namespace Comic.API.Code.Dtos;

public class UserForRegistrationDto
{
	public string FirstName { get; init; }
	public string LastName { get; init; }
	[Required(ErrorMessage = "Email is required")]
	public string Email { get; init; }
	[Required(ErrorMessage = "Password is required")]
	public string Password { get; init; }
    public ICollection<string> Roles { get; init; } = new List<string>() { "User" };
}
