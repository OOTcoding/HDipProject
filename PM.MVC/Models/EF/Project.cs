using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace PM.MVC.Models.EF
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Line length must be between 3 and 50 characters")]
        public string Name { get; set; }

        public string ManagerId { get; set; }

        public IdentityResource Manager { get; set; }

        [Required(ErrorMessage = "Please enter the start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDuration { get; set; }

        [Required(ErrorMessage = "Please enter the end date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDuration { get; set; }

        [JsonIgnore] //Serialization  
        public virtual ICollection<ProjectQualification> ProjectQualifications { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectIdentityResource> ProjectResources { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}