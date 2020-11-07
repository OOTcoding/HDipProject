using System.ComponentModel.DataAnnotations;

namespace PM.MVC.ViewModels
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }
    }
}