namespace WebApi.Code.Database;

public interface IAuditable
{
    DateTime CreatedOn { get; set; }
    int? CreatedBy { get; set; }
    DateTime ChangedOn { get; set; }
    int? ChangedBy { get; set; }
}
