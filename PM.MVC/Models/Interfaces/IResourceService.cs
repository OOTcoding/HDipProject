using System.Collections.Generic;
using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Interfaces
{
    public interface IResourceService<in T>
    {
        Task<IEnumerable<IdentityResource>> GetSuitableResourcesAsync(T source);
        Task AddResourcesAsync(T source, IEnumerable<string> resourcesToAddIds);
        Task DeleteResourceAsync(T source, string resourceId);
    }
}