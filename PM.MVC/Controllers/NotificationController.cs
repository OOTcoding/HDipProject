using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly UserManager<IdentityResource> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(UserManager<IdentityResource> userManager, INotificationRepository notificationRepository)
        {
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        public async Task<IActionResult> GetNotification()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            IEnumerable<NotificationIdentityResource> notificationResult = await _notificationRepository.GetUserNotifications(userId);
            List<NotificationIdentityResource> notification = notificationResult.ToList();
            return Ok(new { UserNotification = notification, notification.Count });
        }

        public async Task<IActionResult> ReadNotification(int notificationId)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            await _notificationRepository.ReadNotification(notificationId, userId);

            return Ok();
        }
    }
}