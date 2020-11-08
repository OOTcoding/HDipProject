using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IQualificationRepository<T>
    {
        //inherits IRepository with CRUD + 2 methods to Add qualification and Delete qualification
        Task<T> AddQualificationAsync(T source, Qualification createRequest);
        Task DeleteQualificationAsync(T source, int qualificationId);
    }
}