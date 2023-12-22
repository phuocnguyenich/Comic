using Comic.API.Code.Dtos;

namespace Comic.API.Code.Interfaces;

public interface IChapterService
{
    Task<ComicDto> GetComicByChapterIdAsync(int chapterId);
    Task<List<string>> GetImagesByChapterId(int chapterId);
}
