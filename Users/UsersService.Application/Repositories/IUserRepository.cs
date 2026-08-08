using UsersService.Domain.Models.Auth;

namespace UsersService.Application.Repositories;

public interface IUserRepository
{
    Task<User?> SelectUserByLoginAsync(string login, CancellationToken ct = default);

    Task<User> InsertUserAsync(User user, CancellationToken ct = default);
}
