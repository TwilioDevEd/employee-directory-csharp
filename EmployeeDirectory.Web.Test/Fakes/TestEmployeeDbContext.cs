using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Moq;
using EmployeeDirectory.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeDirectory.Web.Test.Fakes
{
    public static class TestEmployeeDbContext
    {
        public static EmployeeDbContext GetTestDbContext(IList<Employee> sampleData = null)
        {
            var data = (sampleData ?? new List<Employee>
            {
                new Employee { Id = 1, FullName = "Joe Programmer", Email = "joe@example.com", ImageUrl = "http://example.com/joe.png", PhoneNumber = "12345678901" },
                new Employee { Id = 2, FullName = "Sally Programmer", Email = "sally@example.com", ImageUrl = "http://example.com/sally.png", PhoneNumber = "12345678902" },
                new Employee { Id = 3, FullName = "Code Monkey", Email = "monkey@example.com", ImageUrl = "http://example.com/monkey.png", PhoneNumber = "12345678903" }
            }).AsQueryable();

            var mockSet = new Mock<DbSet<Employee>>();
            mockSet.As<IDbAsyncEnumerable<Employee>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Employee>(data.GetEnumerator()));

            mockSet.As<IQueryable<Employee>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Employee>(data.Provider));

            mockSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Employee>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<EmployeeDbContext>();
            mockContext.Setup(m => m.Employees).Returns(mockSet.Object);

            return mockContext.Object;
        }
    }
}
