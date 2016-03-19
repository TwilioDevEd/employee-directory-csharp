using System.Data.Entity;
using EmployeeDirectory.Web.Models.SeedData;
using SQLite.CodeFirst;

namespace EmployeeDirectory.Web.Models.Sqlite
{
    public class EmployeeDbSqliteInitializer : SqliteCreateDatabaseIfNotExists<EmployeeDbContext>
    {
        protected override void Seed(EmployeeDbContext context)
        {
            base.Seed(context);
            DataSeeder.SeedData(context);
        }

        public EmployeeDbSqliteInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        public EmployeeDbSqliteInitializer(DbModelBuilder modelBuilder, bool nullByteFileMeansNotExisting) : base(modelBuilder, nullByteFileMeansNotExisting)
        {
        }
    }
}