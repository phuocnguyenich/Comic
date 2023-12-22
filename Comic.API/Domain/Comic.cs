using Comic.Api.Code.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comic.API.Domain;

public class Comic : BaseEntity
{
    public string Name { get; set; }
    public string AliasName { get; set; }
    public string CoverUrl { get; set; }

    public string Description { get; set; }
    public ComicStatus Status { get; set; }
    //public long TotalViews { get; set; }
    //public int TotalHearts { get; set; }
    //public int TotalReviews { get; set; }

    public List<CategoryComic> CategoryComics { get; set; }
    public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    public List<AppUser> Users { get; set; }
    public List<AuthorComic> AuthorComics { get; set; }

    public List<DailyComicView> DailyComicViews { get; set; }
    public List<UserRating> UserRatings { get; set; }
    public List<UserComicFollower> Followers { get; set; }

    [NotMapped]
    public double Rating => UserRatings?.Average(x => x.Rating) ?? 0;
    [NotMapped]
    public int TotalRating => UserRatings?.Count ?? 0;
}
