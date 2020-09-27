﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PM.MVC.Models.EF
{
    public class Resource
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Line length must be between 3 and 50 characters")]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<QualificationResource> QualificationResources { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectResource> ProjectResources { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}