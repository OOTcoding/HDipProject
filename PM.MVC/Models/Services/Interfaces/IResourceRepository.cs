using System.Collections.Generic;
using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IResourceRepository<T> : IRepository<T>
    {
        //inherits IRepository with CRUD + 2 methods with resources
        Task<T> AddResourcesAsync(int sourceId, IEnumerable<Resource> resourcesToAdd);
        Task DeleteResourceAsync(int sourceId, int resourceId);
    }
}