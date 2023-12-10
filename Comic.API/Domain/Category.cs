namespace Comic.API.Domain;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public List<CategoryComic> CategoryComics { get; set; }
}
