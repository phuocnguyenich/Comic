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
    public async Task<IActionResult> GetChaptersById(int comicId)
    {
        var result = await _chapterService.GetChaptersById(comicId);
        return Ok(result);
    }
}
