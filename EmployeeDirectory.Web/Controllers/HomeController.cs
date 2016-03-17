using System.Threading.Tasks;
using System.Web.Mvc;
using EmployeeDirectory.Web.Models;
using EmployeeDirectory.Web.Services;

namespace EmployeeDirectory.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
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