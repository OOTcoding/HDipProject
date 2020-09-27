namespace PM.MVC.Models.EF
{
    public class QualificationResource
    {
        public int QualificationId { get; set; }

        public int ResourceId { get; set; }

        public Qualification Qualification { get; set; }

        public Resource Resource { get; set; }
    }
}