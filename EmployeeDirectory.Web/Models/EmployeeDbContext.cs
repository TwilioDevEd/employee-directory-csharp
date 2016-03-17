using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace EmployeeDirectory.Web.Models
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext() : base()
        {
            Database.SetInitializer(new EmployeeDbInitializer());
        }

        public virtual DbSet<Employee> Employees { get; set; }
    }

    class EmployeeDbInitializer : CreateDatabaseIfNotExists<EmployeeDbContext>
    {
        protected override void Seed(EmployeeDbContext context)
        {
            base.Seed(context);
            context.Employees.AddOrUpdate(e => e.Id, new Models.Employee { Id = 1, FullName = "David Prothero", Email = "dprothero@twilio.com", PhoneNumber = "+14155551212", ImageUrl = "http://www.prothero.com/img/portrait500.jpg" });
            context.SaveChanges();
        }
    }
}