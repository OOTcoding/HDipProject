using System.Collections.Generic;
using System.Threading.Tasks;
using PM.MVC.Models.EF;

namespace PM.MVC.Models.Interfaces
{
    public interface IQualificationService<in T>
    {
        Task<IEnumerable<Qualification>> GetSuitableQualificationsAsync(T source);
        Task AddQualificationsAsync(T source, IEnumerable<int> qualificationsToAddIds);
        Task DeleteQualificationAsync(T source, int qualificationId);
    }
}