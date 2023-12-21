using Comic.API.Code.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comic.API.Areas.Image;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("{chapterId}")]
    public async Task<IActionResult> GetImagesByChapterId(int chapterId)
    {
        var result = await _imageService.GetImagesByChapterId(chapterId);
        return Ok(result);
    }
}
