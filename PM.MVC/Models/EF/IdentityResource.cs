using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace PM.MVC.Models.EF
{
    public class IdentityResource : IdentityUser
    {
        [JsonIgnore]
        public virtual ICollection<QualificationIdentityResource> QualificationResources { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectIdentityResource> ProjectResources { get; set; }
    }
}