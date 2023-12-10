using AutoMapper;
using Comic.Api.Code.Enums;
using Comic.API.Code.Dtos;
using Comic.API.Code.Helpers;
using Comic.API.Code.Interfaces;
using Comic.API.Code.Specifications;
using Comic.API.Data;
using Comic.API.Domain;
using Microsoft.EntityFrameworkCore;
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
                         Name = g.Key.Name,
                         AliasName = g.Key.AliasName,
                         CoverUrl = g.Key.CoverUrl,
                         Description = g.Key.Description,
                         Status = g.Key.Status,
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
                     });

        // Use case-insensitive comparison for category
        if (!string.IsNullOrEmpty(comicSpecParams.Category))
        {
            query = query.Where(c => c.Categories.Contains(comicSpecParams.Category));
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
                break;
            case SortOption.Comments:
                break;
            case SortOption.NumberOfChapters:
                query = query.OrderByDescending(x => x.NumberOfChapters);
                break;
            default:
                query = query.OrderByDescending(s => s.ChangedOn);
                break;
        }

        // Additional conditions for search or other parameters can be added here
        var count = await _context.Comics.CountAsync();

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
                           .Where(x => x.Id == id)
                     let chapters = (
                         from ch in _context.Chapters
                         join d in dailyComicViewsQuery on ch.Id equals d.ChapterId
                         where ch.ComicId == c.Id
                         orderby ch.Id descending
                         select new ChapterDto
                         {
                             Id = ch.Id,
                             Name = ch.Name,
                             TotalViews = d.ViewCount,
                             ChangedOn = ch.ChangedOn
                         }
                     ).ToList()

                     select new ComicDto
                     {
                         Id = c.Id,
                         Name = c.Name,
                         AliasName = c.AliasName,
                         CoverUrl = c.CoverUrl,
                         Description = c.Description,
                         Status = c.Status,
                         TotalViews = chapters.Sum(x => x.TotalViews),
                         Rating = c.Rating,
                         TotalRating = c.TotalRating,
                         Authors = c.AuthorComics
                                 .Select(x => x.Author.Name)
                                 .ToArray(),
                         Categories = c.CategoryComics
                                 .Select(x => x.Category.Name)
                                 .ToArray(),
                         Chapters = chapters,
                         ChangedOn = c.ChangedOn
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
                        Status = c.Status,
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
}
