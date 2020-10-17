using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IQualificationRepository<T> : IRepository<T>
    {
        //inherits IRepository with CRUD + 2 methods to Add qualification and Delete qualification
        Task<T> AddQualificationAsync(int id, Qualification createRequest);
        Task DeleteQualificationAsync(int id, int qualificationId);
    }
}