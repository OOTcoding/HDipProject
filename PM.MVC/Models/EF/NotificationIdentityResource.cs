namespace PM.MVC.Models.EF
{
    public class NotificationIdentityResource
    {
        public int NotificationId { get; set; }

        public Notification Notification { get; set; }

        public string IdentityResourceId { get; set; }

        public IdentityResource IdentityResource { get; set; }

        public bool IsRead { get; set; } = false;
    }
}