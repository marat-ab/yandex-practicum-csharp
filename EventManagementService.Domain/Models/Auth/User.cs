using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Models.Auth;

public class User
{
    public User(long id, string login, string passHash, Role role)
    {
        Id = id;
        Login = login;
        PasswordHash = passHash;
        Role = role;
    }

    public long Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
}
