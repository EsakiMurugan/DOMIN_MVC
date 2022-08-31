using DOMIN_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DOMIN_MVC.Controllers
{
    public class ServiceController : Controller
    {
        string baseURL = "https://localhost:7200/";
        //All carts from beginning was display
        public async Task <IActionResult> CartIndex()
        {
            List<Cart>? cart = new List<Cart>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Service");
                if (Res.IsSuccessStatusCode)
                {
                    var CartResponse = Res.Content.ReadAsStringAsync().Result;
                    cart = JsonConvert.DeserializeObject<List<Cart>>(CartResponse);
                }
                ViewData.Model = cart;
            }
            return View();
        }
        public IActionResult AddToCart()
        {
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(Cart cart)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                //Set from Credential controller Login action 
                cart.CustomerID = (int)HttpContext.Session.GetInt32("CustomerID");
                //cart.CustomerID = @ViewBag.CustomerID;
                TempData["CustomerId"] = cart.CustomerID;
                //Set from Pizza controller GetById action
                cart.PizzaID = HttpContext.Session.GetInt32("PizzaID");
                //cart.PizzaID = @ViewBag.PizzaID;    
                StringContent content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Service/AddToCart", content);
                return RedirectToAction("GetMyCart", "Service");
               
            }
        }
        [HttpGet]
        // View Cart by Customer ID (My Cart)
        public async Task<ActionResult<Cart>> GetMyCart()
        {
            List<Cart>? cart = new List<Cart>();
            var id = HttpContext.Session.GetInt32("CustomerID");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Service/" + id);
                if (Res.IsSuccessStatusCode)
                {
                    var apiresponse = Res.Content.ReadAsStringAsync().Result;
                    cart = JsonConvert.DeserializeObject<List<Cart>>(apiresponse);
                }
                var TotalPrice = 0;
                foreach (var p in cart)
                {
                    TotalPrice += (int)p.UnitPrice;
                }
                HttpContext.Session.SetInt32("TotalPrice", TotalPrice);
            }

             return View(cart);
        }
        [HttpGet]
        public async Task<ActionResult> DeleteCart(int id)
        {
            Cart? cart = new Cart();
            TempData["Id"] = id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("api/Service/GetCartById?id=" + id))
                {
                    var apiresponse = response.Content.ReadAsStringAsync().Result;
                    cart = JsonConvert.DeserializeObject<Cart>(apiresponse);
                }
            }
            return View(cart);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteCart(Cart c)
        {
            int id = Convert.ToInt32(TempData["Id"]);
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                await client.DeleteAsync("api/Service/" + id);
                return RedirectToAction("GetMyCart", "Service");
            }
        }
        [HttpGet]
        public async Task<ActionResult> EditCart(int id)
        {
            Cart? cart = new Cart();
            TempData["Id"] = id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("api/Service/GetCartById?id=" + id))
                {
                    var apiresponse = response.Content.ReadAsStringAsync().Result;
                    cart = JsonConvert.DeserializeObject<Cart>(apiresponse);
                }
            }
            return View(cart);
        }
        [HttpPost]
        public async Task<ActionResult> EditCart(Cart cart)
        {
            int id = Convert.ToInt32(TempData["Id"]);
            Cart c1 = new Cart();
            //cart.CustomerID= (int)HttpContext.Session.GetInt32("CustomerID");
            using (var Client = new HttpClient())
            {
               Client.BaseAddress = new Uri(baseURL);
               Client.DefaultRequestHeaders.Clear();
                //int id = (int)cart.CartID;
                StringContent content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8,"application/json");
                using (var response = await Client.PutAsync("api/Service/" + id, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    c1 = JsonConvert.DeserializeObject<Cart>(apiResponse);
                }
            }
            return RedirectToAction("GetMyCart", "Service");
        }


      
    }
    
}
