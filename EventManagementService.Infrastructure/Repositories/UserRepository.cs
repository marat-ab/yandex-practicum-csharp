using EventManagementService.Application.Repositories;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using EventManagementService.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Infrastructure.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbc;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public UserRepository(AppDbContext dbc)
    {
        _dbc = dbc;
    }

    public async Task<User> SelectUserByLoginAsync(string login, CancellationToken ct = default)
    {
        try
        {
            await _semaphore.WaitAsync(ct);

            var result = _dbc.Users
                .Where(x => x.Login == login)
                .FirstOrDefault();

            if (result != null)
            {
                return result;
            }
            else
            {
                throw new UserNotFoundException(login,
                    $"Can't get user with login = {login}. It is absent");
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<User> InsertUserAsync(User user, CancellationToken ct = default)
    {
        try
        {
            await _semaphore.WaitAsync(ct);

            var id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;

            var tmpUser = new User(
                id: id,
                login: user.Login,
                passwordHash: user.PasswordHash,
                role: user.Role);

            await _dbc.Users.AddAsync(tmpUser, ct);
            await _dbc.SaveChangesAsync(ct);

            return tmpUser;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
