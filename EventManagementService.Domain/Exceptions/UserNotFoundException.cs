namespace EventManagementService.Domain.Exceptions;

public sealed class UserNotFoundException : Exception
{
    public string Login { get; init; } = string.Empty;

    public UserNotFoundException()
        :base(message: "User not found error (without user id)")
    {
    }

    public UserNotFoundException(string login, string message)
        : base(message: message)
    {
        Login = login;
    }
}