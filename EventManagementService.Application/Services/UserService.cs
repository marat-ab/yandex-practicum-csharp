using EventManagementService.Application.Repositories;
using EventManagementService.Domain.Models.Auth;
using EventManagementService.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IJWTService _jwtService;
    public UserService(
        IUserRepository userRepository,
        IEncryptionService encryptionService, 
        IJWTService jwtService)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _jwtService = jwtService;
    }

    public async Task<User> CreateUserAsync(string login, string password, Role role, CancellationToken ct = default)
    {
        var passwordHash = _encryptionService.CalcHash(password);

        var newUser = new User(id: Guid.Empty, login: login, passwordHash: passwordHash, role: role);

        var result = await _userRepository.InsertUserAsync(newUser, ct);

        return result;
    }

    public async Task<string?> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var passwordHash = _encryptionService.CalcHash(password);

        var user = await _userRepository.SelectUserByLoginAsync(login, ct);

        if (user is null)
            throw new UnauthorizedAccessException($"User with login {login}, not found");

        if (user.PasswordHash != passwordHash)
            throw new UnauthorizedAccessException($"Bad password for {login}. Access denied");

        var result = _jwtService.CreateJWTToken(user.Id, login, user.Role);

        return result;
    }
}
