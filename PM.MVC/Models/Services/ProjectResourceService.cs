using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Services
{
    public class ProjectResourceService : IResourceService<Project>
    {
        private readonly PMAppDbContext _context;

        public ProjectResourceService(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IdentityResource>> GetSuitableResourcesAsync(Project source)
        {
            IdentityResource[] resources = await _context.IdentityResources.Include(x => x.ProjectResources)
                                                         .ThenInclude(x => x.IdentityResource)
                                                         .Include(x => x.QualificationResources)
                                                         .ThenInclude(x => x.Qualification)
                                                         .ThenInclude(x=>x.Skill)
                                                         .ToArrayAsync();

            var projectQualifications = await _context.ProjectQualifications.Where(x => x.ProjectId == source.Id).ToArrayAsync();

            return resources
                   .Select(resource =>
                               (resource,
                                meetRequirementsQualifications: resource.QualificationResources.Where(x => projectQualifications.Any(d => d.QualificationId == x.QualificationId))
                                                                        .ToList()))
                   .Where(t => t.meetRequirementsQualifications.Count > 0 && t.resource.ProjectResources.All(x => x.ProjectId != source.Id))
                   .Select(t => t.resource);
        }

        public async Task AddResourcesAsync(Project source, IEnumerable<string> resourcesToAddIds)
        {
            IdentityResource[] query = await _context.IdentityResources.Include(x => x.ProjectResources).ToArrayAsync();
            IEnumerable<IdentityResource> resources = query.Where(x => resourcesToAddIds.Any(id => id == x.Id));

            foreach (IdentityResource resource in resources)
            {
                ProjectIdentityResource projectIdentityResource = new ProjectIdentityResource { ProjectId = source.Id, IdentityResourceId = resource.Id };

                await _context.ProjectIdentityResources.AddAsync(projectIdentityResource);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteResourceAsync(Project source, string resourceId)
        {
            ProjectIdentityResource projectIdentityResource = await GetDbProjectResource(source.Id, resourceId);

            _context.ProjectIdentityResources.Remove(projectIdentityResource);
            await _context.SaveChangesAsync();
        }

        private async Task<ProjectIdentityResource> GetDbProjectResource(int projectId, string resourceId)
        {
            var projectResource = await _context.ProjectIdentityResources.Include(x => x.Project)
                                                .Include(x => x.IdentityResource)
                                                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.IdentityResourceId == resourceId);

            if (projectResource == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return projectResource;
        }
    }
}