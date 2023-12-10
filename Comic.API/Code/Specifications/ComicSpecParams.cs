using Comic.Api.Code.Enums;

namespace Comic.API.Code.Specifications;

public class ComicSpecParams
{
    private const int MaxPageSize = 50;
    public int PageIndex { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string Category { get; set; }
    //public int? TypeId { get; set; }
    public SortOption? Sort { get; set; }
    public ComicStatus? Status { get; set; }
    private string _search;
    public string Search
    {
        get => _search;
        set => _search = value.ToLower();
    }
}
