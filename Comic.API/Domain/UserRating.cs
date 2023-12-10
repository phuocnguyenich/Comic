namespace Comic.API.Domain;

public class UserRating : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }

    public int Rating { get; set; }
}
