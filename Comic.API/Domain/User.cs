using Microsoft.AspNetCore.Identity;

namespace Comic.API.Domain;

public class AppUser : IdentityUser<int>
{
    public string DisplayName { get; set; }

    public List<UserRating> Ratings { get; set; }
    public List<Comment> Comments { get; set; }
    public List<UserComicFollower> FollowedComics { get; set; }
}
