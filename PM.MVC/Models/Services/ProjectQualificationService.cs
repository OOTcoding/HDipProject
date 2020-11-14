using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Services
{
    public class ProjectQualificationService : IQualificationService<Project>
    {
        private readonly PMAppDbContext _context;

        public ProjectQualificationService(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Qualification>> GetSuitableQualificationsAsync(Project source)
        {
            var projectQualifications = await _context.ProjectQualifications.Include(x => x.Qualification)
                                                      .ThenInclude(x => x.Skill)
                                                      .Where(x => x.ProjectId == source.Id)
                                                      .ToArrayAsync();
            var qualifications = await _context.Qualifications.ToArrayAsync();

            return qualifications.Where(qualification => projectQualifications.All(p => p.QualificationId != qualification.Id));
        }

        public async Task AddQualificationsAsync(Project source, IEnumerable<int> qualificationsToAddIds)
        {
            foreach (int qualificationId in qualificationsToAddIds)
            {
                ProjectQualification projectQualification = new ProjectQualification { ProjectId = source.Id, QualificationId = qualificationId };
                await _context.ProjectQualifications.AddAsync(projectQualification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteQualificationAsync(Project source, int qualificationId)
        {
            ProjectQualification projectQualification = await GetDbProjectQualification(source.Id, qualificationId);

            _context.ProjectQualifications.Remove(projectQualification);
            await _context.SaveChangesAsync();
        }

        private async Task<ProjectQualification> GetDbProjectQualification(int projectId, int qualificationId)
        {
            var projectQualification = await _context.ProjectQualifications.Include(x => x.Project)
                                                     .Include(x => x.Qualification)
                                                     .FirstOrDefaultAsync(x => x.QualificationId == qualificationId && x.ProjectId == projectId);

            if (projectQualification == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return projectQualification;
        }
    }
}