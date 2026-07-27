using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Infrastructure.Models;

public class JWTSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationTimeMinutes { get; set; }
}
