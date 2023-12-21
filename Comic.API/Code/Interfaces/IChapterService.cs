using Comic.API.Code.Dtos;

namespace Comic.API.Code.Interfaces;

public interface IChapterService
{
    Task<List<ChapterDto>> GetChaptersById(int id);
}
