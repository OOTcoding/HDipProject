using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM.MVC.Models.Interfaces
{
    public interface IRepository<T>
    {
        //Implements basic CRUD methods
        Task<T> GetOneAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T createRequest);
        Task<T> UpdateAsync(T updateRequest);
        Task DeleteAsync(int id);
    }
}