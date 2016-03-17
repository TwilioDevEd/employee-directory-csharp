using EmployeeDirectory.Web.Models;
using EmployeeDirectory.Web.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Twilio.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;
using System.Collections.Generic;
using System;

namespace EmployeeDirectory.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public const string NotFoundMessage = "We could not find an employee matching '{0}'";
        public const string CookieName = "EmpDirIdList";
        private EmployeeDirectoryService _service;

        public EmployeeController() : base()
        {
            _service = new EmployeeDirectoryService(new EmployeeDbContext());
        }

        public EmployeeController(EmployeeDirectoryService service) : base()
        {
            _service = service;
        }

        // POST: Employee/Lookup
        [HttpPost]
        public async Task<ActionResult> Lookup(SmsRequest request)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            var num = ParseNumber(request.Body);
            var keys = Request.Cookies.Get(CookieName)?.Value?.Split('~');
            if (num > 0 && keys != null && num <= keys.Length)
            {
                var id = ParseNumber(keys[num - 1]);
                var employee = await _service.FindByIdAsync(id);
                if (employee != null) employees = new[] { employee };
            }
            else
            {
                employees = await _service.FindByNamePartialAsync(request.Body);
            }

            var response = GetTwilioResponseForEmployees(employees.ToList(), request.Body);
            return new TwiMLResult(response);
        }

        private int ParseNumber(string body)
        {
            int value;
            if(int.TryParse(body, out value))
            {
                return value;
            }
            return 0;
        }

        private TwilioResponse GetTwilioResponseForEmployees(List<Employee> employees, string body)
        {
            var response = new TwilioResponse();

            switch (employees.Count)
            {
                case 0:
                    response.Message(string.Format(NotFoundMessage, body));
                    break;

                case 1:
                    var employee = employees.First();
                    response.Message(employee.FullName + " - " + employee.Email + " - " + employee.PhoneNumber, new string[] { employee.ImageUrl }, null);
                    break;

                default:
                    response.Message(GetMessageForMultipleEmployees(employees));
                    break;
            }

            return response;
        }

        private string GetMessageForMultipleEmployees(List<Employee> employees)
        {
            var msg = "We found: ";
            var idList = "";
            for (var i = 0; i < employees.Count; i++)
            {
                msg += (i+1) + "-" + employees[i].FullName;
                idList += employees[i].Id;
                if (i < employees.Count - 1)
                {
                    msg += ", ";
                    idList += "~";
                }
            }
            msg += " - Reply with # of desired person";

            var cookie = new System.Web.HttpCookie(CookieName, idList);
            cookie.Expires = DateTime.UtcNow.AddDays(1);
            Response.Cookies.Add(cookie);

            return msg;
        }
    }
}
