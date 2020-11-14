using System.Collections.Generic;
using System.Data;

namespace PM.MVC.Models.Interfaces
{
    public interface IExcelService<in T>
    {
        DataTable GetTable(IEnumerable<T> resources);
    }
}