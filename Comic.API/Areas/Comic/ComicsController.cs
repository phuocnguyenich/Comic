using Comic.API.Code.Dtos;
using Comic.API.Code.Interfaces;
using Comic.API.Code.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Comic;

[Route("api/[controller]")]
[ApiController]
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

    [HttpGet("getRecommendComics")]
    public async Task<IActionResult> GetRecommendComics()
    {
        var paginationResult = await _comicService.GetRecommendComicsAsync();
        return Ok(paginationResult);
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
}
