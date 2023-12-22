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
}
