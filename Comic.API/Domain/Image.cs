namespace Comic.API.Domain;

public class Image : BaseEntity
{
    public string Name { get; set; }
    public string Url { get; set; }

    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}
