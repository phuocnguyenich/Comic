using Comic.API.Code.Dtos;
using Comic.API.Domain;

namespace Comic.API.Code.Interfaces;

public interface IImageService
{
    Task<List<string>> GetImagesByChapterId(int chapterId);
}
