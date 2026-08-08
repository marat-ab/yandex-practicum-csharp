using System;
using System.Collections.Generic;
using System.Text;

namespace EventsService.Domain.Models;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}
