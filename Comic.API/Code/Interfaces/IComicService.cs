using Comic.API.Code.Dtos;
using Comic.API.Code.Helpers;
using Comic.API.Code.Specifications;

namespace Comic.API.Code.Interfaces;

public interface IComicService
{
    Task<Pagination<ComicDto>> GetComicsAsync(ComicSpecParams comicSpecParams);
    Task<List<ComicDto>> GetRecommendComicsAsync();
    Task<ComicDto> GetComicByIdAsync(int id);
}
