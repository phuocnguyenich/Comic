using Comic.API.Code.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Chapter;

[Route("api/[controller]")]
[ApiController]
public class ChaptersController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChaptersController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet("{chapterId}/comic")]
    public async Task<IActionResult> GetComicByChapterId(int chapterId)
    {
        var comic = await _chapterService.GetComicByChapterIdAsync(chapterId);
        if (comic == null)
        {
            return NotFound();
        }
        return Ok(comic);
    }

    [HttpGet("{chapterId}/images")]
    public async Task<IActionResult> GetImagesByChapterId(int chapterId)
    {
        var result = await _chapterService.GetImagesByChapterId(chapterId);
        return Ok(result);
    }
}
