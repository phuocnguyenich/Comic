namespace Comic.API.Domain;

public class AuthorComic : BaseEntity
{
    public int AuthorId { get; set; }
    public Author Author { get; set; }

    public int ComicId { get; set; }
    public Comic Comic { get; set; }
}
