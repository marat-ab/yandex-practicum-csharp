using EventManagementService.Application.Repositories;
using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Services;

internal class UserService : IUserService
{
    public readonly IUserRepository _userRepository;
    public readonly IJWTService _jwtService;
    public UserService(
        IUserRepository userRepository, 
        IJWTService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<User> CreateUserAsync(string login, string passwordHash, Role role, CancellationToken ct)
    {
        var newUser = new User(id: Guid.Empty, login: login, passwordHash: passwordHash, role: role);

        var result = await _userRepository.InsertUserAsync(newUser, ct);

        return result;
    }

    public async Task<string?> LoginAsync(string login, string passwordHash, CancellationToken ct)
    {
        var user = await _userRepository.SelectUserByLoginAsync(login, ct);

        if (user is null)
            throw new UnauthorizedAccessException($"User with login {login}, not found");

        if (user.PasswordHash != passwordHash)
            throw new UnauthorizedAccessException($"Bad password for {login}. Access denied");

        var result = _jwtService.CreateJWTToken(user.Id, login, user.Role);

        return result;
    }
}
