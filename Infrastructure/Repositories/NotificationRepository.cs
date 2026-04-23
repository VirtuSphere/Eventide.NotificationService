using Eventide.NotificationService.Domain.Entities;
using Eventide.NotificationService.Domain.Interfaces;
using Eventide.NotificationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventide.NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context) => _context = context;

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId, bool unreadOnly, int skip, int take, CancellationToken ct)
    {
        var query = _context.Notifications.Where(n => n.RecipientUserId == userId);
        if (unreadOnly) query = query.Where(n => !n.IsRead);
        return await query.OrderByDescending(n => n.CreatedAt).Skip(skip).Take(take).ToListAsync(ct);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Notifications.FindAsync(new object[] { id }, ct);

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct)
        => await _context.Notifications.CountAsync(n => n.RecipientUserId == userId && !n.IsRead, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct)
        => await _context.Notifications.AddAsync(notification, ct);

    public Task UpdateAsync(Notification notification, CancellationToken ct)
    { _context.Notifications.Update(notification); return Task.CompletedTask; }

    public async Task SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);
}