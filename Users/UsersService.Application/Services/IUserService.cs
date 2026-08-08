using UsersService.Domain.Models.Auth;

namespace UsersService.Application.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(string login, string password, Role role, CancellationToken ct = default);

    Task<string?> LoginAsync(string login, string password, CancellationToken ct = default);
}
