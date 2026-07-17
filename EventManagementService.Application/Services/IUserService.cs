using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(string login, string password, Role role, CancellationToken ct = default);

    Task<string?> LoginAsync(string login, string password, CancellationToken ct = default);
}
