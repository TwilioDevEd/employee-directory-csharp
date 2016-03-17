using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDirectory.Web.Models;

namespace EmployeeDirectory.Web.Services
{
    public class EmployeeDirectoryService
    {
        private EmployeeDbContext _context;

        public EmployeeDirectoryService(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> FindByNamePartialAsync(string partialName)
        {
            return await _context.Employees.Where(e => e.FullName.Contains(partialName)).ToListAsync();
        }

        public async Task<Employee> FindByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
