using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PM.MVC.Models.EF
{
    public class Qualification
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        public Skill Skill { get; set; }

        public int SkillId { get; set; }

        public SkillLevel Level { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectQualification> ProjectQualification { get; set; }

        [JsonIgnore]
        public virtual ICollection<QualificationResource> QualificationResources { get; set; }

        public override string ToString()
        {
            return $"{Skill.Name} - {Level}";
        }
    }
}