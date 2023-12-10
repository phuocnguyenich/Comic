using WebApi.Code.Database;

namespace Comic.API.Domain;

public class BaseEntity : IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime ChangedOn { get; set; }
    public int? ChangedBy { get; set; }
}
