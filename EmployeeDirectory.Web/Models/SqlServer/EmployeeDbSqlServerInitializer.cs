using System.Data.Entity;
using EmployeeDirectory.Web.Models.SeedData;

namespace EmployeeDirectory.Web.Models.SqlServer
{
    public class EmployeeDbSqlServerInitializer : CreateDatabaseIfNotExists<EmployeeDbContext>
    {
        protected override void Seed(EmployeeDbContext context)
        {
            base.Seed(context);
            DataSeeder.SeedData(context);
        }
    }
}