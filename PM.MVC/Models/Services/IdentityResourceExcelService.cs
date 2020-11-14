using System.Collections.Generic;
using System.Data;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Services
{
    public class IdentityResourceExcelService : IExcelService<IdentityResource>
    {
        public DataTable GetTable(IEnumerable<IdentityResource> resources)
        {
            var table = new DataTable { TableName = "Resources" };

            table.Columns.Add("Id");
            table.Columns["Id"].DataType = typeof(string);
            table.Columns.Add("Email");

            foreach (IdentityResource identityResource in resources)
            {
                FillRows(table, identityResource);
            }

            return table;
        }

        private static void FillRows(DataTable table, IdentityResource identityResource)
        {
            DataRow row = table.NewRow();

            row["Id"] = identityResource.Id;
            row["Email"] = identityResource.Email;

            table.Rows.Add(row);
        }
    }
}