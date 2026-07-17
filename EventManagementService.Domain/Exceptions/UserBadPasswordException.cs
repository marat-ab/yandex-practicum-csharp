namespace EventManagementService.Domain.Exceptions;

public sealed class UserBadPasswordException : Exception
{
    public string Login { get; init; } = string.Empty;

    public UserBadPasswordException()
        :base(message: "User wrong password error (without user id)")
    {
    }

    public UserBadPasswordException(string login, string message)
        : base(message: message)
    {
        Login = login;
    }
}