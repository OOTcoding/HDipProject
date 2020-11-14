using System.Collections.Generic;

namespace PM.MVC.ViewModels
{
    public class ChartDataViewModel
    {
        public ChartDataViewModel()
        {
            ChartData = new Dictionary<string, int>();
        }

        public Dictionary<string, int> ChartData { get; set; }

        public int ProjectsCount { get; set; }
        public int ResourcesCount { get; set; }
        public int QualificationsCount { get; set; }
        public int SkillsCount { get; set; }

        public string Label { get; set; }

        public ButtonType ButtonType { get; set; }
    }

    public enum ButtonType
    {
        Project, Qualification, Resource, Skill
    }
}