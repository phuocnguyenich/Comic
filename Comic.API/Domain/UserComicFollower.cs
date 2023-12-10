namespace Comic.API.Domain;

public class UserComicFollower : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }
}
