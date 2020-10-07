using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetOneAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T createRequest);
        Task<T> UpdateAsync(T updateRequest);
        Task DeleteAsync(int id);
    }
}