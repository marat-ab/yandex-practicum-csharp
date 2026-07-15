using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Models.Auth;

public class User
{
    public User(Guid id, string login, string passwordHash, Role role)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }

    public List<Booking> Bookings { get; set; }
}
