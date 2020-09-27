namespace PM.MVC.Models.EF
{
    public class ProjectQualification
    {
        public int ProjectId { get; set; }

        public int QualificationId { get; set; }

        public Project Project { get; set; }

        public Qualification Qualification { get; set; }
    }
}