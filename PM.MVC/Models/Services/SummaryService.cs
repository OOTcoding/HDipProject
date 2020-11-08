using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.ViewModels;

namespace PM.MVC.Models.Services
{
    public class SummaryService
    {
        private readonly PMAppDbContext _context;

        public SummaryService(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<ChartDataViewModel> GetProjectsPerManagerChartViewModelAsync()
        {
            var qualifications = await _context.Qualifications.ToArrayAsync();
            var resources = await _context.IdentityResources.ToArrayAsync();
            var skills = await _context.Skills.ToArrayAsync();

            var projects = await _context.Projects.Include(x => x.Manager).ToArrayAsync();
            Dictionary<string, int> query = GetProjectsPerManager(projects);

            return new ChartDataViewModel
            {
                ChartData = query,
                Label = "Projects per manager",
                ProjectsCount = projects.Length,
                QualificationsCount = qualifications.Length,
                ResourcesCount = resources.Length,
                SkillsCount = skills.Length
            };
        }

        public async Task GetProjectsPerManagerAsync(ChartDataViewModel viewModel)
        {
            var projects = await _context.Projects.Include(x => x.Manager).ToArrayAsync();
            viewModel.ChartData = GetProjectsPerManager(projects);
            viewModel.Label = "Projects by manager";
        }

        public async Task GetQualificationsByProjectAsync(ChartDataViewModel viewModel)
        {
            var projectQualifications = await _context.ProjectQualifications.Include(x => x.Project).ToArrayAsync();
            viewModel.ChartData = projectQualifications.GroupBy(x => x.Project.Name, x => x.QualificationId).ToDictionary(x => x.Key, x => x.Count());
            viewModel.Label = "Qualifications by project";
        }

        public async Task GetResourcesByProjectAsync(ChartDataViewModel viewModel)
        {
            var projectResources = await _context.ProjectIdentityResources.Include(x => x.Project).ToArrayAsync();
            viewModel.ChartData = projectResources.GroupBy(x => x.Project.Name, x => x.IdentityResourceId).ToDictionary(x => x.Key, x => x.Count());
            viewModel.Label = "Resources by project";
        }

        public async Task GetSkillsByLevelAsync(ChartDataViewModel viewModel)
        {
            var qualifications = await _context.Qualifications.Include(x => x.Skill).ToArrayAsync();
            viewModel.ChartData = qualifications.GroupBy(x => x.Level, x => x.Skill).ToDictionary(x => x.Key.ToString(), x => x.Count());
            viewModel.Label = "Skills by level";
        }

        private static Dictionary<string, int> GetProjectsPerManager(IEnumerable<Project> projects)
        {
            Dictionary<string, int> query = projects.GroupBy(x => x.Manager.UserName, x => x.Name).ToDictionary(x => x.Key.ToString(), x => x.Count());
            return query;
        }
    }
}