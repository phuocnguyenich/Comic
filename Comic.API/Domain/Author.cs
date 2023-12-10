namespace Comic.API.Domain;

public class Author : BaseEntity
{
    public string Name { get; set; }
    public List<AuthorComic> AuthorComics { get; set; }
}
