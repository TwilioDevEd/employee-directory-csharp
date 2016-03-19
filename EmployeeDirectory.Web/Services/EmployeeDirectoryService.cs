using EmployeeDirectory.Web.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDirectory.Web.Services
{
    public class EmployeeDirectoryService
    {
        private readonly EmployeeDbContext _context;

        public EmployeeDirectoryService(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> FindByNamePartialAsync(string partialName)
        {
            partialName = partialName.ToLower();
            return await _context.Employees
                .Where(e => e.FullName.ToLower().Contains(partialName))
                .ToListAsync();
        }

        public async Task<Employee> FindByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
