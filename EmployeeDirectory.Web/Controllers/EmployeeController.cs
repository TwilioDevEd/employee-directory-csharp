using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeDirectory.Web.Models;
using EmployeeDirectory.Web.Services;
using Twilio.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace EmployeeDirectory.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public const string NotFoundMessage = "We could not find an employee matching '{0}'";
        public const string CookieName = "EmpDirIdList";
        private readonly EmployeeDirectoryService _service;

        public EmployeeController()
        {
            _service = new EmployeeDirectoryService(new EmployeeDbContext());
        }

        public EmployeeController(EmployeeDirectoryService service)
        {
            _service = service;
        }

        // Twilio will call this whenever our phone # receives an SMS message.
        [HttpPost]
        public async Task<ActionResult> Lookup(SmsRequest request)
        {
            var incomingMessageText = request.Body;

            var employees = await GetEmployeesIfNumericInput(incomingMessageText) ??
                            await _service.FindByNamePartialAsync(incomingMessageText);

            var response = GetTwilioResponseForEmployees(employees, incomingMessageText);
            return new TwiMLResult(response);
        }

        private async Task<IEnumerable<Employee>> GetEmployeesIfNumericInput(string incomingMessageText)
        {
            var num = ParseNumber(incomingMessageText);
            var keys = GetListOfKeysFromCookie();

            if (IsNumberAndInListOfKeys(num, keys))
            {
                return await GetListOfOneEmployeeById(keys, num);
            }

            return null;
        }

        private static int ParseNumber(string incomingMessageText)
        {
            int value;
            return int.TryParse(incomingMessageText, out value) ? value : 0;
        }

        private string[] GetListOfKeysFromCookie()
        {
            var cookie = Request.Cookies.Get(CookieName);
            if (cookie == null || cookie.Value == null) return null;
            return cookie.Value.Split('~');
        }

        private static bool IsNumberAndInListOfKeys(int num, IReadOnlyList<string> keys)
        {
            return num > 0 && keys != null && num <= keys.Count;
        }

        private async Task<IEnumerable<Employee>> GetListOfOneEmployeeById(IReadOnlyList<string> keys, int num)
        {
            var id = ParseNumber(keys[num - 1]);
            var employee = await _service.FindByIdAsync(id);
            return employee == null ? null : ConvertToArray(employee);
        }

        private static IEnumerable<Employee> ConvertToArray(Employee employee)
        {
            return new [] { employee };
        }

        private TwilioResponse GetTwilioResponseForEmployees(IEnumerable<Employee> employees, string incomingMessageText)
        {
            var response = new TwilioResponse();
            var employeeList = employees.ToList();

            switch (employeeList.Count)
            {
                case 0: // No Employees Found
                    response.Message(string.Format(NotFoundMessage, incomingMessageText));
                    break;

                case 1: // A Single Employee Found
                    var employee = employeeList.First();
                    response.Message(employee.FullName + " - " + employee.Email + " - " + employee.PhoneNumber,
                        new[] {employee.ImageUrl}, null);
                    break;

                default: // Multiple Employees Found
                    response.Message(GetMessageForMultipleEmployees(employeeList));
                    break;
            }

            return response;
        }

        private string GetMessageForMultipleEmployees(IReadOnlyList<Employee> employees)
        {
            var msg = "We found: ";
            var idList = "";
            for (var i = 0; i < employees.Count; i++)
            {
                msg += i + 1 + "-" + employees[i].FullName;
                idList += employees[i].Id;
                if (i == employees.Count - 1) continue;
                msg += ", ";
                idList += "~";
            }
            msg += " - Reply with # of desired person";

            AddListOfIdsCookie(idList);
            return msg;
        }

        private void AddListOfIdsCookie(string idList)
        {
            var cookie = new HttpCookie(CookieName, idList)
            {
                Expires = DateTime.UtcNow.AddHours(4)
            };
            Response.Cookies.Add(cookie);
        }
    }
}