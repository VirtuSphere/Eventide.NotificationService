namespace Eventide.NotificationService.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public DateTime CreatedAt { get; init; }
}