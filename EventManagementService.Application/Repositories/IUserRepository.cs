using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Repositories;

public interface IUserRepository
{
    Task<User> SelectUserByLoginAsync(string login, CancellationToken ct = default);

    Task<User> InsertUserAsync(User user, CancellationToken ct = default);
}
