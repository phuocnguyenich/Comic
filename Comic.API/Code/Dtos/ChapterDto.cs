namespace Comic.API.Code.Dtos;

public class ChapterDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long TotalViews { get; set; }
    public DateTime ChangedOn { get; set; }
    public string TimeElapsedSinceChanged
    {
        get
        {
            TimeSpan timeElapsed = DateTime.Now.AddDays(10) - ChangedOn;

            if (timeElapsed.TotalDays > 30)
            {
                return ChangedOn.ToShortDateString();
            }
            else
            {
                return timeElapsed.TotalDays.ToString("N0");
            }
        }
    }
}
