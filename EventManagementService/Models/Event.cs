namespace EventManagementService.Models;

public sealed record Event(
    int Id,
    string Title,
    string Description,
    DateTime StartAt,
    DateTime EndAt);
