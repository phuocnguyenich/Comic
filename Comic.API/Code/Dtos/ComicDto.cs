using Comic.Api.Code.Enums;

namespace Comic.API.Code.Dtos;

public class ComicDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AliasName { get; set; }
    public string CoverUrl { get; set; }
    public string Description { get; set; }
    public ComicStatus Status { get; set; }
    public long TotalViews { get; set; }
    public int TotalHearts { get; set; }
    public double Rating { get; set; }
    public int TotalRating { get; set; }
    public int NumberOfChapters { get; set; }
    public string[] Categories { get; set; }
    public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();
    public string[] Authors { get; set; }
    public DateTime ChangedOn { get; set; }
}
