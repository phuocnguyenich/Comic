using Comic.Api.Code.Enums;
using Comic.API.Code.Extensions;
using Comic.API.Domain;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text;

namespace Comic.API.Code.Dtos;

public class ComicDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AliasName { get; set; }
    public string CoverUrl { get; set; }
    public string Description { get; set; }
    public ComicStatus StatusId { get; set; }
    public string Status => StatusId.GetDescription();
    public long TotalViews { get; set; }
    public double Rating { get; set; }
    public int TotalRating { get; set; }
    public int NumberOfChapters { get; set; }
    public CategoryDto[] Categories { get; set; }
    public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();
    public AuthorDto[] Authors { get; set; }
    public DateTime ChangedOn { get; set; }
    public string FormattedChangedOn => ChangedOn.ToString("HH:mm dd/MM/yyyy");
    public long TotalFollowers { get; set; }
    public long TotalComments { get; set; }
    public string UnsignedName
    {
        get
        {
            return Name.RemoveAccents();
        }
    }
}
