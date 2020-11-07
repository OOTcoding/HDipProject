using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PM.MVC.ViewModels
{
    public class EditProjectViewModel
    {
        public EditProjectViewModel()
        {
            Users = new List<IdentityUser>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Line length must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDuration { get; set; }

        [Required(ErrorMessage = "Please enter the end date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDuration { get; set; }

        [Display(Name = "Manager")]
        public string ManagerId { get; set; }

        public List<IdentityUser> Users { get; set; }
    }
}