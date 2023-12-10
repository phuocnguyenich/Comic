namespace Comic.API.Domain;

public class Comment : BaseEntity
{
    public string Content { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; }
    //public List<Comic> Comics { get; set; }
    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}
