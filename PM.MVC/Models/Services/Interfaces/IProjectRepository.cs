using System.Collections.Generic;
using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IProjectRepository : IQualificationRepository<Project>, IResourceRepository<Project>
    {
        Task<IEnumerable<Resource>> GetSuitableResourcesAsync(int projectId);
        Task<IEnumerable<Qualification>> GetSuitableQualificationsAsync(int projectId);
        Task<Project> AddQualificationsAsync(int projectId, IEnumerable<Qualification> qualificationsToAdd);
    }
}