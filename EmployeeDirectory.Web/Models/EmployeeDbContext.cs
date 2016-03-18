using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Web;

namespace EmployeeDirectory.Web.Models
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext() : base("DefaultConnection")
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
            context.Employees.AddOrUpdate(e => e.Id, new Employee { Id = 1, FullName = "David Prothero", Email = "dprothero@twilio.com", PhoneNumber = "+14155551212", ImageUrl = "http://gravatar.com/avatar/560630d6945c6a8777533e7ea3b4e69b?s=200" });

            // Data provided by Marvel. © 2016 MARVEL
            var seedDataPath = HttpContext.Current?.Server.MapPath("~/App_Data/seed-data.json");
            if(!string.IsNullOrEmpty(seedDataPath) && File.Exists(seedDataPath))
            {
                var exampleRecords = JsonConvert.DeserializeObject<Employee[]>(File.ReadAllText(seedDataPath));
                foreach(var record in exampleRecords)
                {
                    context.Employees.AddOrUpdate(e => e.Id, record);
                }
            }

            context.SaveChanges();
        }
    }
}