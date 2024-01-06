using AutoMapper;
using Comic.Api.Code.Enums;
using Comic.API.Code.Dtos;
using Comic.API.Code.Extensions;
using Comic.API.Code.Helpers;
using Comic.API.Code.Interfaces;
using Comic.API.Code.Specifications;
using Comic.API.Data;
using Comic.API.Domain;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Comic.API.Code.Services;

public class ComicService : IComicService
{
    private readonly ComicDbContext _context;
    private readonly IMapper _mapper;

    public ComicService(ComicDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Other methods...

    public async Task<Pagination<ComicDto>> GetComicsAsync(ComicSpecParams comicSpecParams)
    {
        IQueryable<Domain.Comic> comicQuery = _context.Comics;

        // Extracted common filtering logic
        comicQuery = FilterComics(comicQuery, comicSpecParams.Status);

        IQueryable<DailyComicView> dailyComicViewsQuery = _context.DailyComicViews;

        // Extracted common filtering logic
        dailyComicViewsQuery = FilterDailyComicViews(dailyComicViewsQuery, comicSpecParams.Sort);

        
        var query = (from c in comicQuery
                     join ch in _context.Chapters on c.Id equals ch.ComicId
                     join d in dailyComicViewsQuery on ch.Id equals d.ChapterId into dailyComicViewsGroup
                     from d1 in dailyComicViewsGroup.DefaultIfEmpty()
                     group new { c, d1, ch } by new 
                     { c.Id, c.Name, c.AliasName, 
                         c.CoverUrl, c.Description, 
                         c.Status, c.ChangedOn }
                     into g
                    // let latestChapters = (
                    //    from ch in _context.Chapters
                    //    where ch.ComicId == g.Key.Id
                    //    orderby ch.Id descending
                    //    select new ChapterDto
                    //    {
                    //        Id = ch.Id,
                    //        Name = ch.Name
                    //    }
                    //).Take(3).ToList()
                     select new ComicDto
                     {
                         Id = g.Key.Id,
                         Name = "Con đường bá chủ",
                         AliasName = g.Key.AliasName,
                         CoverUrl = "https://st.nettruyenus.com/data/comics/32/vo-luyen-dinh-phong-9068.jpg",
                         Description = g.Key.Description,
                         StatusId = g.Key.Status,
                         TotalViews = g.Sum(x => x.d1.ViewCount),
                         //Rating = g.Key.Rating,
                         //TotalRating = g.Key.TotalRating,
                         Chapters = g.Select(x => new ChapterDto
                         {
                             Id = x.ch.Id,
                             Name = x.ch.Name,
                             ChangedOn = x.ch.ChangedOn,
                         }).OrderByDescending(x => x.Id).Take(3).ToList(),
                         ChangedOn = g.Key.ChangedOn,
                         NumberOfChapters = g.Select(x => x.ch.Id).Count(),
                         TotalFollowers = _context.Followers.Count(x => x.ComicId == g.Key.Id),
                         TotalComments = _context.Comments.Count(x => g.Select(a => a.ch.Id).Contains(x.ChapterId)),
                     });

        // Use case-insensitive comparison for category
        if (!string.IsNullOrEmpty(comicSpecParams.Category))
        {
            query = query.Where(c => c.Categories.Select(x => x.Name).Contains(comicSpecParams.Category));
        }

        switch (comicSpecParams.Sort)
        {
            case SortOption.NewComic:
                break;
            case SortOption.TopAll:
            case SortOption.TopMonth:
            case SortOption.TopWeek:
            case SortOption.TopDay:
                query = query.OrderByDescending(s => s.TotalViews);
                break;
            case SortOption.Follow:
                query = query.OrderByDescending(s => s.TotalFollowers);
                break;
            case SortOption.Comments:
                query = query.OrderByDescending(s => s.TotalComments);
                break;
            case SortOption.NumberOfChapters:
                query = query.OrderByDescending(x => x.NumberOfChapters);
                break;
            default:
                query = query.OrderByDescending(s => s.ChangedOn);
                break;
        }

        // Additional conditions for search or other parameters can be added here
        var count = await (from c in comicQuery
                           join ch in _context.Chapters on c.Id equals ch.ComicId
                           join d in dailyComicViewsQuery on ch.Id equals d.ChapterId into dailyComicViewsGroup
                           from d1 in dailyComicViewsGroup.DefaultIfEmpty()
                           group c by c.Id into g
                           select g.Key)
                           .CountAsync();

        // Combine Count and Data Retrieval
        var comics = await query
            .Skip((comicSpecParams.PageIndex - 1) * comicSpecParams.PageSize)
            .Take(comicSpecParams.PageSize)
            .ToListAsync();

        //var comicDtos = _mapper.Map<List<ComicDto>>(comics);

        return new Pagination<ComicDto>(comicSpecParams.PageIndex, comicSpecParams.PageSize, count, comics);
    }

    private IQueryable<Domain.Comic> FilterComics(IQueryable<Domain.Comic> query, ComicStatus? status)
    {
        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status);
        }

        return query;
    }

    private IQueryable<DailyComicView> FilterDailyComicViews(IQueryable<DailyComicView> query, SortOption? sort)
    {
        SortOption[] sortOptions = { 
            SortOption.TopAll, 
            SortOption.TopMonth, 
            SortOption.TopWeek, 
            SortOption.TopDay 
        };

        if (sort.HasValue && sortOptions.Contains(sort.Value))
        {
            var today = DateTime.Now;

            query = query.Where(d =>
                (sort == SortOption.TopAll) ||
                (sort == SortOption.TopMonth && d.ViewDate.Month == today.Month) ||
                (sort == SortOption.TopWeek && EF.Functions.DateDiffWeek(d.ViewDate, today) == 0) ||
                (sort == SortOption.TopDay && d.ViewDate.Day == today.Day));
        }

        return query.GroupBy(d => d.ChapterId)
                .Select(g => new DailyComicView
                {
                    ChapterId = g.Key,
                    ViewCount = g.Sum(d => d.ViewCount)
                });
    }

    public async Task<ComicDto> GetComicByIdAsync(int id)
    {
        var dailyComicViewsQuery = _context.DailyComicViews.GroupBy(d => d.ChapterId)
                .Select(g => new DailyComicView
                {
                    ChapterId = g.Key,
                    ViewCount = g.Sum(d => d.ViewCount)
                });

        var comic = await (from c in _context.Comics
                           .Include(x => x.UserRatings)
                           .Include(x => x.Followers)
                           .Where(x => x.Id == id)
                     let chapters = (
                         from ch in _context.Chapters
                         join d in dailyComicViewsQuery on ch.Id equals d.ChapterId into dailyComicViewsGroup
                         from d1 in dailyComicViewsGroup.DefaultIfEmpty()
                         where ch.ComicId == c.Id
                         orderby ch.Id descending
                         select new ChapterDto
                         {
                             Id = ch.Id,
                             Name = ch.Name,
                             TotalViews = (long?)d1.ViewCount ?? 0,
                             ChangedOn = ch.ChangedOn
                         }
                     )
                     .OrderByDescending(x => x.Id)
                     .ToList()

                     select new ComicDto
                     {
                         Id = c.Id,
                         Name = "Con đường bá chủ",
                         AliasName = c.AliasName,
                         CoverUrl = "https://st.nettruyenus.com/data/comics/32/vo-luyen-dinh-phong-9068.jpg",
                         Description = c.Description,
                         StatusId = c.Status,
                         TotalViews = chapters.Sum(x => x.TotalViews),
                         Rating = c.Rating,
                         TotalRating = c.TotalRating,
                         Authors = c.AuthorComics
                                 .Select(x => new AuthorDto
                                 {
                                     Id = x.Author.Id,
                                     Name = x.Author.Name
                                 }).ToArray(),
                         Categories = c.CategoryComics
                                 .Select(x => new CategoryDto
                                 {
                                     Id = x.Category.Id,
                                     Name = x.Category.Name
                                 }).ToArray(),
                         Chapters = chapters,
                         ChangedOn = c.ChangedOn,
                         TotalFollowers = c.Followers.Count,
                     }).FirstOrDefaultAsync();

        //return _mapper.Map<ComicDto>(comic);

        return comic;
    }

    public async Task<List<ComicDto>> GetRecommendComicsAsync()
    {
        var query = from c in _context.Comics
                    let latestChapters = (
                        from ch in _context.Chapters
                        where ch.ComicId == c.Id
                        orderby ch.Id descending
                        select new ChapterDto
                        {
                            Id = ch.Id,
                            Name = ch.Name
                        }
                    ).Take(1).ToList()
                    select new ComicDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        AliasName = c.AliasName,
                        CoverUrl = c.CoverUrl,
                        Description = c.Description,
                        StatusId = c.Status,
                        //TotalViews = c.TotalViews,
                        //TotalHearts = c.TotalHearts,
                        //Rating = c.Rating,
                        //TotalRating = c.TotalRating,
                        ChangedOn = c.ChangedOn,
                        Chapters = latestChapters
                    };

        // You can add similar conditions for other parameters like TypeId, Search, etc.
        return await query.Take(12).ToListAsync();

        //return _mapper.Map<List<ComicDto>>(comics);
    }

    public async Task<List<ChapterDto>> GetChaptersByComicId(int comicId)
    {
        return await _context.Chapters
            .Where(x => x.ComicId == comicId)
            .Select(x => new ChapterDto
            {
                Id = x.Id,
                Name = x.Name,
                ChangedOn = x.ChangedOn
            })
            .ToListAsync();
    }
}
