using Comic.API.Code.Interfaces;
using Comic.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Comic.API.Code.Services;

public class ImageService : IImageService
{
    private readonly ComicDbContext _context;

    public ImageService(ComicDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetImagesByChapterId(int chapterId)
    {
        return await _context.Images
                    .Where(x => x.ChapterId == chapterId)
                    .Select(x => x.Url)
                    .ToListAsync();
    }
}
