using Microsoft.AspNetCore.Mvc;
using MvcApiProject.Models;
using SharedModels.Models;
using System.Diagnostics;

namespace MvcApiProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient { BaseAddress = new Uri("https://localhost:7060") };
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostCity(string city, string countryCode)
        {
            var url = $"api/Country?city={city}&countryCode={countryCode}";
            try
            {
                var countries = await client.GetFromJsonAsync<List<CountrySelector>>(url);
                if (countries != null)
                {
                    return RedirectToAction("City", new { lat = countries[0].lat, lon = countries[0].lon, countryCode = countryCode, city = city });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(null);
        }

        public async Task<IActionResult> City(double lat, double lon, string countryCode, string city)
        {
            var url = $"api/Weather?lat={lat}&lon={lon}";
            try
            {
                ViewData["city"] = city;
                ViewData["countryCode"] = countryCode;
                var weatherData = await client.GetFromJsonAsync<WeatherData>(url);
                if (weatherData != null)
                {
                    return View(weatherData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(null);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}