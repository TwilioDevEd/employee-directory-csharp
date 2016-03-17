using EmployeeDirectory.Web.Controllers;
using EmployeeDirectory.Web.Services;
using EmployeeDirectory.Web.Test.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Twilio.Mvc;
using Twilio.TwiML.Mvc;

namespace EmployeeDirectory.Web.Test.Controllers
{
    [TestClass]
    public class EmployeeControllerTest
    {
        private HttpCookieCollection _requestCookies = new HttpCookieCollection();
        private HttpCookieCollection _responseCookies = new HttpCookieCollection();

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_not_found_for_unknown_name()
        {
            var xml = await GetXmlResultFromLookupAsync("unknown");
            var expectedMessage = string.Format(EmployeeController.NotFoundMessage, "unknown");
            Assert.IsTrue(xml.Contains(expectedMessage));
        }

        private async Task<string> GetXmlResultFromLookupAsync(string lookupString)
        {
            var ctrl = GetTestController();
            var result = (await ctrl.Lookup(new SmsRequest { Body = lookupString })) as TwiMLResult;
            Assert.IsNotNull(result);

            return GetXmlFromTwiMLResult(result);
        }

        private EmployeeController GetTestController()
        {
            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();

            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.Response).Returns(response.Object);
            request.Setup(r => r.Cookies).Returns(_requestCookies);
            response.Setup(r => r.Cookies).Returns(_responseCookies);

            var ctrl = new EmployeeController(new EmployeeDirectoryService(TestEmployeeDbContext.GetTestDbContext()));
            ctrl.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), ctrl);
            return ctrl;
        }

        private string GetXmlFromTwiMLResult(TwiMLResult result)
        {
            var field = typeof(TwiMLResult).GetField("data", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var xml = field.GetValue(result).ToString();
            Trace.WriteLine(xml);
            return xml;
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_joe_record()
        {
            var xml = await GetXmlResultFromLookupAsync("Joe Programmer");
            Assert.IsTrue(xml.Contains("Joe Programmer"));
            Assert.IsTrue(xml.Contains("joe@example.com"));
            Assert.IsTrue(xml.Contains("joe.png"));
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_multiple_records()
        {
            var xml = await GetXmlResultFromLookupAsync("Programmer");
            Assert.IsTrue(xml.Contains("Joe Programmer"));
            Assert.IsFalse(xml.Contains("joe@example.com"));
            Assert.IsTrue(xml.Contains("Sally Programmer"));

            var cookie = _responseCookies.Get(EmployeeController.CookieName);
            Assert.IsNotNull(cookie);
            Assert.AreEqual("1~2", cookie.Value);
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_not_found_for_numeric_but_no_cookie()
        {
            var xml = await GetXmlResultFromLookupAsync("1");
            var expectedMessage = string.Format(EmployeeController.NotFoundMessage, "1");
            Assert.IsTrue(xml.Contains(expectedMessage));
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_not_found_for_numeric_but_null_cookie()
        {
            _requestCookies.Add(new HttpCookie(EmployeeController.CookieName, null));
            var xml = await GetXmlResultFromLookupAsync("1");
            var expectedMessage = string.Format(EmployeeController.NotFoundMessage, "1");
            Assert.IsTrue(xml.Contains(expectedMessage));
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_not_found_for_numeric_but_invalid_cookie()
        {
            _requestCookies.Add(new HttpCookie(EmployeeController.CookieName, "alskdfjsadf"));
            var xml = await GetXmlResultFromLookupAsync("1");
            var expectedMessage = string.Format(EmployeeController.NotFoundMessage, "1");
            Assert.IsTrue(xml.Contains(expectedMessage));
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_joe_for_numeric_with_good_cookie()
        {
            _requestCookies.Add(new HttpCookie(EmployeeController.CookieName, "1~2"));
            var xml = await GetXmlResultFromLookupAsync("1");
            Assert.IsTrue(xml.Contains("Joe Programmer"));
            Assert.IsTrue(xml.Contains("joe@example.com"));
            Assert.IsTrue(xml.Contains("joe.png"));
        }

        [TestMethod]
        public async Task EmployeeController_Lookup_should_return_sally_for_numeric_with_good_cookie()
        {
            _requestCookies.Add(new HttpCookie(EmployeeController.CookieName, "2~3~1"));
            var xml = await GetXmlResultFromLookupAsync("1");
            Assert.IsTrue(xml.Contains("Sally Programmer"));
            Assert.IsTrue(xml.Contains("sally@example.com"));
            Assert.IsTrue(xml.Contains("sally.png"));
        }
    }
}
