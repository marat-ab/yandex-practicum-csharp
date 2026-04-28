namespace EventManagementService.Models;

public sealed record Event(
    Guid Id,
    string Title,
    string Description,
    DateTime StartAt,
    DateTime EndAt);
