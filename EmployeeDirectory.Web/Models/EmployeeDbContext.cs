using System.Configuration;
using System.Data.Entity;
using EmployeeDirectory.Web.Models.Sqlite;
using EmployeeDirectory.Web.Models.SqlServer;

namespace EmployeeDirectory.Web.Models
{
    public class EmployeeDbContext : DbContext
    {
        private const string ConnectionStringName = "DefaultConnection";

        public EmployeeDbContext() : base(ConnectionStringName)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Use the correct initializer will be used based on the providerName
            // for the 'DefaultConnection' connectionString.
            var cs = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (cs.ProviderName == "System.Data.SQLite")
            {
                Database.SetInitializer(new EmployeeDbSqliteInitializer(modelBuilder));
            }
            else
            {
                Database.SetInitializer(new EmployeeDbSqlServerInitializer());
            }
        }
    }
}