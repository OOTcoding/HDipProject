using System.Collections.Generic;

namespace PM.MVC.Models.EF
{
    public class Notification
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual ICollection<NotificationIdentityResource> NotificationIdentityResources { get; set; }
    }
}