namespace EventManagementService.Domain.Exceptions;

public sealed class UserRoleNotFoundException : Exception
{
    public long UserId { get; init; }

    public UserRoleNotFoundException()
        :base(message: "User role not found error (without user id)")
    {
    }

    public UserRoleNotFoundException(long userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}