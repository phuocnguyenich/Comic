using Comic.API.Code.Dtos;
using Comic.API.Code.Interfaces;
using Comic.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Comic.API.Code.Services;

public class ChapterService : IChapterService
{
    private readonly ComicDbContext _context;

    public ChapterService(ComicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChapterDto>> GetChaptersById(int id)
    {
        var comicId = await _context.Chapters
            .Where(x => x.Id == id)
            .Select(x => x.ComicId)
            .FirstOrDefaultAsync();

        return await _context.Chapters
                .Where(x => x.ComicId == comicId)
                .Select(x => new ChapterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();
    }
}
