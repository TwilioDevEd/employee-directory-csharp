using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeDirectory.Web.Services;
using EmployeeDirectory.Web.Test.Fakes;
using System.Collections.Generic;
using EmployeeDirectory.Web.Models;

namespace EmployeeDirectory.Web.Test.Services
{
    [TestClass]
    public class EmployeeDirectoryServiceTest
    {
        [TestMethod]
        public async Task EmployeeDirectoryService_FindByIdAsync_should_return_null_for_unknown_id()
        {
            var service = new EmployeeDirectoryService(TestEmployeeDbContext.GetTestDbContext());
            var result = await service.FindByIdAsync(9999);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByIdAsync_should_return_joe_for_id_1()
        {
            var service = new EmployeeDirectoryService(TestEmployeeDbContext.GetTestDbContext());
            var result = await service.FindByIdAsync(1);
            AssertEmployeeIs("Joe Programmer", result);
        }

        private void AssertEmployeeIs(string fullName, Employee employee)
        {
            Assert.IsNotNull(employee);
            Assert.AreEqual(fullName, employee.FullName);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_joe_for_full_name()
        {
            var results = await FindByNamePartial("Joe Programmer");
            AssertEmployeeListOnlyContains("Joe Programmer", results);
        }

        private async Task<IList<Employee>> FindByNamePartial(string partialName)
        {
            var service = new EmployeeDirectoryService(TestEmployeeDbContext.GetTestDbContext());
            var results = await service.FindByNamePartialAsync(partialName);
            return results.ToList();
        }

        private void AssertEmployeeListOnlyContains(string fullName, IList<Employee> employees)
        {
            Assert.IsNotNull(employees);
            Assert.AreEqual(1, employees.Count);
            AssertEmployeeListContains(fullName, employees);
        }

        private void AssertEmployeeListContains(string fullName, IList<Employee> employees)
        {
            var employee = employees.FirstOrDefault(e => e.FullName == fullName);
            Assert.IsNotNull(employee);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_joe_for_first_name()
        {
            var results = await FindByNamePartial("Joe");
            AssertEmployeeListOnlyContains("Joe Programmer", results);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_joe_for_partial()
        {
            var results = await FindByNamePartial("oe Pro");
            AssertEmployeeListOnlyContains("Joe Programmer", results);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_monkey_for_last_name()
        {
            var results = await FindByNamePartial("Monkey");
            AssertEmployeeListOnlyContains("Code Monkey", results);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_monkey_for_last_name_lowercase()
        {
            var results = await FindByNamePartial("monkey");
            AssertEmployeeListOnlyContains("Code Monkey", results);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_programmers_for_last_name()
        {
            var results = await FindByNamePartial("Programmer");
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            AssertEmployeeListContains("Joe Programmer", results);
            AssertEmployeeListContains("Sally Programmer", results);
        }

        [TestMethod]
        public async Task EmployeeDirectoryService_FindByNamePartialAsync_should_return_empty_for_unknown()
        {
            var results = await FindByNamePartial("unknown");
            Assert.AreEqual(0, results.Count);
        }

    }
}
