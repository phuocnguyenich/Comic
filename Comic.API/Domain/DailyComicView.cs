namespace Comic.API.Domain;

public class DailyComicView
{
    public int Id { get; set; }
    public DateTime ViewDate { get; set; }
    public long ViewCount { get; set; }

    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
}
