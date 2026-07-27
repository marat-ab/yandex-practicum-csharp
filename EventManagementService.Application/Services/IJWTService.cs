using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Application.Services;

public interface IJWTService
{
    string CreateJWTToken(Guid userId, string login, Role role);
}
