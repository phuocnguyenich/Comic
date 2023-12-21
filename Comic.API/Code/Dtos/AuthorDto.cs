using Comic.API.Code.Extensions;

namespace Comic.API.Code.Dtos;

public class AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UnsignedName
    {
        get
        {
            return Name.RemoveAccents();
        }
    }
}
