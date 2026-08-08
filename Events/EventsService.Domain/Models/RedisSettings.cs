using System;
using System.Collections.Generic;
using System.Text;

namespace EventsService.Domain.Models;

public class RedisSettings
{
    public string RedisServer { get; set; } = string.Empty;

    public int EventsTtlSeconds { get; set; }
}
