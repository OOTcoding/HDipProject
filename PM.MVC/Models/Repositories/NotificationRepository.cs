using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Infrastructure;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PMAppDbContext _context;
        private readonly IHubContext<SignalServer> _hubContext;

        public NotificationRepository(PMAppDbContext context, IHubContext<SignalServer> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationIdentityResource>> GetUserNotifications(string userId)
        {
            return await _context.UserNotifications.Where(x => x.IdentityResourceId == userId && !x.IsRead).Include(x => x.Notification).ToListAsync();
        }

        public async Task Create(Notification notification, IEnumerable<string> userIdsToNotify)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            var userNotifications = userIdsToNotify.Select(userId => new NotificationIdentityResource
            {
                IdentityResourceId = userId, NotificationId = notification.Id
            });

            await _context.UserNotifications.AddRangeAsync(userNotifications);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("displayNotification");
        }

        public async Task ReadNotification(int notificationId, string userId)
        {
            NotificationIdentityResource notification = await _context.UserNotifications.FirstOrDefaultAsync(x => x.IdentityResourceId == userId && x.NotificationId == notificationId);

            if (notification != null)
            {
                _context.UserNotifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}