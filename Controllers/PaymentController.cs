using DOMIN_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DOMIN_MVC.Controllers
{
    public class PaymentController : Controller
    {
        string baseURL = "https://localhost:7200/";

        public IActionResult Pay()
        {
            return View();
        }
        [HttpPost]
        //Cart Items are purchased and proceed to payment
        public async Task<IActionResult> Pay(Payment payment)
        {
            Payment p = new Payment();
            p.CustomerID = (int)HttpContext.Session.GetInt32("CustomerID");
            p.CardNumber = payment.CardNumber;
            Receipt receipt = new Receipt();
            receipt.CustomerID = (int)HttpContext.Session.GetInt32("CustomerID");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);  
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync("api/Payment/Payment",content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    payment = JsonConvert.DeserializeObject<Payment>(apiResponse);
                }
                //All purchased items from different Customers are added (For Admin to check total items selled)
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(receipt), Encoding.UTF8, "application/json");
                await client.PostAsync("api/Payment/Order", content1);
            }
            return RedirectToAction("Receipt");
        }
        //Receipt View
        public async Task<IActionResult> Receipt()
        {
            var ID = HttpContext.Session.GetInt32("CustomerID");
            List<Cart> cart = new List<Cart>();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("api/Service/" + ID);
                if (res.IsSuccessStatusCode)
                {
                    var cartresponse = res.Content.ReadAsStringAsync().Result;
                    cart = JsonConvert.DeserializeObject<List<Cart>>(cartresponse);
                }
            }
            List<Payment> payment = new List<Payment>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("api/Payment/GetPaymentById?id=" + ID);
                if (res.IsSuccessStatusCode)
                {
                    var payresponse = res.Content.ReadAsStringAsync().Result;
                    payment = JsonConvert.DeserializeObject<List<Payment>>(payresponse);
                }
            }
            var LastList = payment.LastOrDefault();

            List<Pizza> pizza = new List<Pizza>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("api/Pizza");
                if (res.IsSuccessStatusCode)
                {
                    var payresponse = res.Content.ReadAsStringAsync().Result;
                    pizza = JsonConvert.DeserializeObject<List<Pizza>>(payresponse);
                }
            }
            int count = cart.Count();
            var a = new List<object>();
            foreach(var item in cart)
            {
                foreach(var item1 in pizza)
                {
                    if (item.PizzaID == item1.PizzaID)
                        a.Add(item1.PizzaName);
                       
                }
            }
            var b = new List<object>();
            foreach (var item in cart)
            {
                foreach (var item1 in pizza)
                {
                    if (item.PizzaID == item1.PizzaID)
                        b.Add(item1.Price);
                }
            }
            int TotalAmount = 0;
            var c = new List<object>();
            foreach (var item in cart)
            {
                TotalAmount += (int)item.UnitPrice;
                c.Add(item.Quantity);
                HttpContext.Session.SetInt32("TotalAmount", TotalAmount);
            }

            ViewBag.Collection = a;
            ViewBag.Collection1 = b;
            ViewBag.Collection2 = c;
            ViewBag.TA = HttpContext.Session.GetInt32("TotalAmount");

            //DateTime date = DateTime.Today.AddDays(10);
            //string Today = date.ToString("dd/MM/yyyy");
            //ViewBag.Date = date;
            //ViewBag.Total = HttpContext.Session.GetInt32("t");
            //ViewBag.UserName = HttpContext.Session.GetString("UserName");
            //ViewBag.Location = HttpContext.Session.GetString("Location");
            //ViewBag.PhNum = HttpContext.Session.GetString("PhoneNumber");
            //ViewBag.EmailId = HttpContext.Session.GetString("EmailId");
            // return View();

            //Update CartType ID after Payment
            Customer customer = new Customer();
            customer.CustomerID = (int)ID;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/Service/UpdateTypeId", content);
            }
            return View();
        }
        [HttpGet]
        //Already Purchased Items are Viewed here
        public async Task<ActionResult<Receipt>> GetMyOrders()
        {
            string Token = HttpContext.Session.GetString("Token");
            List<Receipt>? receipt = new List<Receipt>();
            var id = HttpContext.Session.GetInt32("CustomerID");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Payment/GetMyOrders?id=" + id);
                if (Res.IsSuccessStatusCode)
                {
                    var apiresponse = Res.Content.ReadAsStringAsync().Result;
                    receipt = JsonConvert.DeserializeObject<List<Receipt>>(apiresponse);
                }
                return View(receipt);
            }
        }
    }

        
    
}
