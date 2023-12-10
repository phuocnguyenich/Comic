using Comic.API.Code.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Image;

[Route("api/[controller]")]
[ApiController]
public class ChaptersController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChaptersController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet("{comicId}")]
    public async Task<IActionResult> GetComicsAsync(int comicId)
    {
        var result = await _chapterService.GetChaptersByComicId(comicId);
        return Ok(result);
    }
}
