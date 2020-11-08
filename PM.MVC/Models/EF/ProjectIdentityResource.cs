namespace PM.MVC.Models.EF
{
    public class ProjectIdentityResource
    {
        public int ProjectId { get; set; }

        public string IdentityResourceId { get; set; }

        public Project Project { get; set; }

        public IdentityResource IdentityResource { get; set; }
    }
}