using Comic.API.Code.Extensions;

namespace Comic.API.Code.Dtos;

public class ChapterDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public long TotalViews { get; set; }
    public string FormattedTotalViews => TotalViews.FormatAsVietnameseNumber();
    public DateTime? ChangedOn { get; set; }
    public string TimeElapsedSinceChanged
    {
        get
        {
            if (ChangedOn == null) return null;

            TimeSpan timeElapsed = DateTime.UtcNow - ChangedOn.Value;

            if (timeElapsed.TotalHours < 1)
            {
                return timeElapsed.TotalHours.ToString("N0") + " phút trước";
            }
            if (timeElapsed.TotalHours < 24)
            {
                return timeElapsed.TotalHours.ToString("N0") + " giờ trước";
            }
            if (timeElapsed.TotalDays < 30)
            {
                return timeElapsed.TotalDays.ToString("N0") + " ngày trước";
            }

            return ChangedOn.Value.ToShortDateString();
        }
    }

    public string UnsignedName
    {
        get
        {
            return Name.RemoveAccents();
        }
    }
}
