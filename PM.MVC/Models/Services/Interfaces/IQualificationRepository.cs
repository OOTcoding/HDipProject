using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Services.Interfaces
{
    public interface IQualificationRepository<T> : IRepository<T>
    {
        Task<T> AddQualificationAsync(int id, Qualification createRequest);
        Task DeleteQualificationAsync(int id, int qualificationId);
    }
}