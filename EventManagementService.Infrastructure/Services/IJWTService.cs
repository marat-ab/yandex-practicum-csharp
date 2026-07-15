using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Infrastructure.Services;

public interface IJWTService
{
    string CreateJWTToken(long userId, string login, Role role);
}
