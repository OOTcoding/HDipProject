using System.Collections.Generic;
using System.Data;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Services
{
    public class QualificationExcelService : IExcelService<Qualification>
    {
        public DataTable GetTable(IEnumerable<Qualification> qualifications)
        {
            var table = new DataTable { TableName = "Qualifications" };

            table.Columns.Add("Id");
            table.Columns["Id"].DataType = typeof(int);
            table.Columns.Add("Skill");
            table.Columns.Add("Level");

            foreach (Qualification qualification in qualifications)
            {
                FillRows(table, qualification);
            }

            return table;
        }

        private static void FillRows(DataTable table, Qualification qualification)
        {
            DataRow row = table.NewRow();

            row["Id"] = qualification.Id;
            row["Skill"] = qualification.Skill.Name;
            row["Level"] = qualification.Level.ToString();

            table.Rows.Add(row);
        }
    }
}