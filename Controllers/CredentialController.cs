using DOMIN_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DOMIN_MVC.Controllers
{
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var canAcess = false;

            // check the refer
            var referer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                var rUri = new System.UriBuilder(referer).Uri;
                var req = filterContext.HttpContext.Request;
                if (req.Host.Host == rUri.Host && req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme)
                {
                    canAcess = true;
                }
            }

            // ... check other requirements

            if (!canAcess)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
            }
        }
    }
    public class CredentialController : Controller
    {
        string baseURL = "https://localhost:7200/";
        public IActionResult CustomerRegn()
        {
            return View();
        }
        [HttpPost]
        [NoDirectAccess]
        public async Task<IActionResult> CustomerRegn(Customer? customer)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();   
                StringContent content = new StringContent(JsonConvert.SerializeObject(customer),Encoding.UTF8,"application/json");
                var response = await client.PostAsync("api/Credential/CustomerRegn", content);

                var FromEmail = new MailAddress("pizzashop97@gmail.com", "Domino's shop");
                var ToEmail = new MailAddress(customer.EmailID, "Receiver");
                var password = "kqubsqgzhqzbvxvq";
                var sub = "Registration Details";
                var body = "Hello" + " " + customer.CustomerName + "!" + "You are successfully registered." + "Your password is" + " " + customer.Password;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(FromEmail.Address, password)
                };
                using (var message = new MailMessage(FromEmail, ToEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return RedirectToAction("CustomerLogin","Credential");
            }
        }
        public IActionResult CustomerLogin()
        {
            return View();
        }
        [HttpPost]
        [NoDirectAccess]
        public async Task<IActionResult> CustomerLogin(Customer? customer)
        {
            JWT jwt = new JWT();
            customer.CPassword = customer.Password;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("api/Credential/CustomerLogin", content);
                if (Res.IsSuccessStatusCode)
                {
                    string apiResponse = await Res.Content.ReadAsStringAsync();
                    jwt = JsonConvert.DeserializeObject<JWT>(apiResponse);
                    if(jwt == null)
                    {
                        ViewBag.ErrorMessage = "*Invalid Email-ID or Password";
                        return View();
                    }
                    HttpContext.Session.SetString("Customer", customer.EmailID);
                    HttpContext.Session.SetInt32("CustomerID", jwt.customer.CustomerID);
                    string Token = jwt.Token;
                    HttpContext.Session.SetString("Token",Token);
                    return RedirectToAction("Get", "Pizza");
                }
                return RedirectToAction("CustomerLogin");
            }
        }
        public IActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        [NoDirectAccess]
        public async Task<IActionResult> AdminLogin(Admin admin)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(admin), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Credential/AdminLogin", content);
                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("Admin", admin.EmailID);
                    return RedirectToAction("Get", "Pizza");
                }
                else
                {
                    ViewBag.ErrorMessageA = "*Invalid Email-ID or Password";
                    //return RedirectToAction("AdminLogin");
                    return View();
                }
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        
    }
}
