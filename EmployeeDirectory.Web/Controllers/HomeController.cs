using EmployeeDirectory.Web.Models;
using EmployeeDirectory.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeDirectory.Web.Controllers
{
    public class HomeController : Controller
    {
        // The Home controller is just to validate that our app is working.
        public async Task<ActionResult> Index()
        {
            using (var context = new EmployeeDbContext())
            {
                var service = new EmployeeDirectoryService(context);
                ViewBag.FirstEmployee = await service.FindByIdAsync(1);
            }

            return View();
        }
    }
}