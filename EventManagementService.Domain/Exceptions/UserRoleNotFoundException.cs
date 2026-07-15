namespace EventManagementService.Domain.Exceptions;

public sealed class UserRoleNotFoundException : Exception
{
    public Guid UserId { get; init; }

    public UserRoleNotFoundException()
        :base(message: "User role not found error (without user id)")
    {
    }

    public UserRoleNotFoundException(Guid userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}