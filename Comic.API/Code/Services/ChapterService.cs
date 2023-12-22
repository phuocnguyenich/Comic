using AutoMapper;
using Comic.API.Code.Dtos;
using Comic.API.Code.Interfaces;
using Comic.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Comic.API.Code.Services;

public class ChapterService : IChapterService
{
    private readonly ComicDbContext _context;
    private readonly IMapper _mapper;

    public ChapterService(ComicDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ComicDto> GetComicByChapterIdAsync(int chapterId)
    {
        var comic = await _context.Chapters
            .Where(x => x.Id == chapterId)
            .Select(x => x.Comic)
            .FirstOrDefaultAsync();

        return _mapper.Map<ComicDto>(comic);
    }
    
    public async Task<List<string>> GetImagesByChapterId(int chapterId)
    {
        return await _context.Images
            .Where(x => x.ChapterId == chapterId)
            .Select(x => new
            {
                Url = "https://p2.ntcdntempv26.com/content/image.jpg?data=lYQzNlIkekgNptSkjz69cPPM34wuv1kLztTrI9j8ohOZIycoDpSXdv7KonquWc2h+UIvOKL5vK7oV00DTb931w=="
            })
            .Select(x => x.Url)
            .ToListAsync();
    }
}
