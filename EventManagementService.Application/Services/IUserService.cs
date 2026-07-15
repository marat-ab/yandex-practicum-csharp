using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(string login, string passwordHash, Role role, CancellationToken ct);

    Task<string?> LoginAsync(string login, string passwordHash, CancellationToken ct);
}
