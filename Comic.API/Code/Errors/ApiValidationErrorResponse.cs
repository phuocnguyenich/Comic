namespace Comic.Api.Code.Errors;

public class ApiValidationErrorResponse : ApiResponse
{
    public ApiValidationErrorResponse() : base(400)
    {
    }

    public List<string> Errors { get; set; }
}