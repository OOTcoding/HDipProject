namespace PM.MVC.Models.EF
{
    public class QualificationIdentityResource
    {
        public int QualificationId { get; set; }

        public string IdentityResourceId { get; set; }

        public Qualification Qualification { get; set; }

        public IdentityResource IdentityResource { get; set; }
    }
}