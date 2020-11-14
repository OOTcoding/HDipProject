using System.Collections.Generic;
using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<NotificationIdentityResource>> GetUserNotifications(string userId);

        Task Create(Notification notification, IEnumerable<string> userIdsToNotify);

        Task ReadNotification(int notificationId, string userId);
    }
}