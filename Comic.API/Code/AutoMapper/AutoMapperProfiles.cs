using AutoMapper;
using Comic.API.Code.Dtos;
using Comic.API.Domain;

namespace Comic.API.Code.AutoMapper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Domain.Comic, ComicDto>();
        CreateMap<Domain.Category, CategoryDto>();
        CreateMap<Domain.Chapter, ChapterDto>();
        CreateMap<Domain.Author, AuthorDto>();

        CreateMap<UserForRegistrationDto, AppUser>();
    }
}
