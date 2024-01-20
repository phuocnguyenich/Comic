namespace Comic.API.Code.Interfaces
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        bool IsUser { get; }

        int? Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
    }
}
