namespace Comic.API.Domain;

public class CategoryComic : BaseEntity
{
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }
}
