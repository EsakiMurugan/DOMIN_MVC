using DOMIN_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text;

namespace DOMIN_MVC.Controllers
{
    public class PizzaController : Controller
    {
        string baseURL = "https://localhost:7200/";
        public async Task<ActionResult<Pizza>> Get()
        {
            List<Pizza> pizza = new List<Pizza>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                //Passing service base url  
                client.DefaultRequestHeaders.Clear();
                //Define request data format 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient 
                HttpResponseMessage Res = await client.GetAsync("api/Pizza");
                //Checking the response is csuccessful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api  
                    var PizResponse = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the product list  
                    pizza = JsonConvert.DeserializeObject<List<Pizza>>(PizResponse);
                }
            }
            return View(pizza);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            Pizza? pizza = new Pizza();
            ViewBag.PizzaID = id;
            HttpContext.Session.SetInt32("PizzaID", id);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("api/Pizza/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    pizza = JsonConvert.DeserializeObject<Pizza>(apiResponse);
                }
            }
            return View(pizza);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Pizza pizza)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(pizza), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Pizza", content);
            }
            return RedirectToAction("Get");
        }
        public async Task<IActionResult> Edit(int id)
        {
            Pizza? p = new Pizza();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("api/Pizza/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    p = JsonConvert.DeserializeObject<Pizza>(apiResponse);
                }
            }
            return View(p);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Pizza pizza)
        {
            Pizza? p = new Pizza();
            using (var client = new HttpClient())
            {
                int id = p.PizzaID;
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(pizza), Encoding.UTF8, "application/json");

                using (var response = await client.PutAsync("api/Pizza", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    p = JsonConvert.DeserializeObject<Pizza>(apiResponse);
                }
            }
            return RedirectToAction("Get");
        }
        public async Task<IActionResult> Delete(int id)
        {
            TempData["Id"] = id;
            Pizza? p = new Pizza();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                using (var response = await client.GetAsync("api/Pizza/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    p = JsonConvert.DeserializeObject<Pizza>(apiResponse);
                }
            }
            return View(p);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Pizza pizza)
        {
            int Id = Convert.ToInt32(TempData["Id"]);
            //Employee emp = new Employee();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Clear();
                await client.DeleteAsync("api/Pizza/" + Id);
               
            }
            return RedirectToAction("Get");
        }
    }
}
