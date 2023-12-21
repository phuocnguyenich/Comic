namespace Comic.API.Domain;

public class Chapter : BaseEntity
{
    public string Name { get; set; }

    public List<Image> Images { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }

    public List<Comment> Comments { get; set; }
    public List<DailyComicView> DailyComicViews { get; set; }
}
