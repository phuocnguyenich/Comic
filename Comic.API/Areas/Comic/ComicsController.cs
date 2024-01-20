using Comic.API.Code.Dtos;
using Comic.API.Code.Interfaces;
using Comic.API.Code.Middleware;
using Comic.API.Code.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Comic;

[Route("api/[controller]")]
[ApiController]
[Cached(600)]
public class ComicsController : ControllerBase
{
    private readonly IComicService _comicService;

    public ComicsController(IComicService comicService)
    {
        _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
    }

    [HttpGet]
    public async Task<IActionResult> GetComicsAsync([FromQuery] ComicSpecParams comicParams)
    {
        var paginationResult = await _comicService.GetComicsAsync(comicParams);
        return Ok(paginationResult);
    }

    [HttpGet("recommended-comics")]
    public async Task<IActionResult> GetRecommendComics()
    {
        var result = await _comicService.GetRecommendComicsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComicByIdAsync(int id)
    {
        var comic = await _comicService.GetComicByIdAsync(id);
        if (comic == null)
        {
            return NotFound();
        }
        return Ok(comic);
    }

    [HttpGet("{comicId}/chapters")]
    public async Task<IActionResult> GetChaptersByComicId(int comicId)
    {
        var result = await _comicService.GetChaptersByComicId(comicId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchComics([FromQuery] string searchTerm)
    {
        var result = await _comicService.SearchComicsAsync(searchTerm);
        return Ok(result);
    }
}
