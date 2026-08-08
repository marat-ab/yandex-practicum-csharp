using UsersService.Domain.Models.Auth;

namespace UsersService.Application.Services;

public interface IJWTService
{
    string CreateJWTToken(Guid userId, string login, Role role);
}
