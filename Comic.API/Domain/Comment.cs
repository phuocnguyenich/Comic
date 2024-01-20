namespace Comic.API.Domain;

public class Comment : BaseEntity
{
    public string Content { get; set; }
    public int? ReplyingTo { get; set; }
    public string LikedBy { get; set; }
    public string DislikedBy { get; set; }
    public List<Comment> ChildComments { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; }
    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}
