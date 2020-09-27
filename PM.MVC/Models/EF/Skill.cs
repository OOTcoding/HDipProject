using System.ComponentModel.DataAnnotations;

namespace PM.MVC.Models.EF
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field must be set")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}