namespace PM.MVC.Models.EF
{
    public class ProjectResource
    {
        public int ProjectId { get; set; }

        public int ResourceId { get; set; }

        public Project Project { get; set; }

        public Resource Resource { get; set; }
    }
}