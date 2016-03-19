using System.Data.Entity.Migrations;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace EmployeeDirectory.Web.Models.SeedData
{
    public class DataSeeder
    {
        public static void SeedData(EmployeeDbContext context)
        {
            AddDavid(context);
            AddSeedDataFromJson(context);

            context.SaveChanges();
        }

        private static void AddDavid(EmployeeDbContext context)
        {
            context.Employees.AddOrUpdate(e => e.Id, new Employee
            {
                Id = 1,
                FullName = "David Prothero",
                Email = "dprothero@twilio.com",
                PhoneNumber = "+14155551212",
                ImageUrl = "http://gravatar.com/avatar/560630d6945c6a8777533e7ea3b4e69b?s=200"
            });
        }

        private static void AddSeedDataFromJson(EmployeeDbContext context)
        {
            // Data provided by Marvel. © 2016 MARVEL
            if (HttpContext.Current == null) return;
            var seedDataPath = HttpContext.Current.Server.MapPath("~/App_Data/seed-data.json");
            if (!IsValidFilePath(seedDataPath)) return;

            var fileContents = File.ReadAllText(seedDataPath);
            var exampleRecords = GetArrayOfEmployeesFromJson(fileContents);
            foreach (var record in exampleRecords)
            {
                context.Employees.AddOrUpdate(e => e.Id, record);
            }
        }

        private static bool IsValidFilePath(string seedDataPath)
        {
            return !string.IsNullOrEmpty(seedDataPath) 
                   && File.Exists(seedDataPath);
        }

        private static Employee[] GetArrayOfEmployeesFromJson(string fileContents)
        {
            return JsonConvert.DeserializeObject<Employee[]>(fileContents);
        }
    }
}