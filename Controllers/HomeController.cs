using Microsoft.AspNetCore.Mvc;
using MvcApiProject.Models;
using SharedModels;
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
        public async Task<IActionResult> PostCity(string city, string countryCode, int days)
        {
            var url = $"api/v1/Country?city={city}&countryCode={countryCode}";
            try
            {
                var countries = await client.GetFromJsonAsync<List<CountrySelector>>(url);
                if (countries != null)
                {
                    return RedirectToAction("City", new { lat = countries[0].lat, lon = countries[0].lon, countryCode = countryCode, city = city, days = days });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(null);
        }

        public async Task<IActionResult> City(double lat, double lon, string countryCode, string city, string days)
        {
            var url = $"api/v1/Weather?lat={lat}&lon={lon}";
            try
            {
                ViewData["city"] = city;
                ViewData["countryCode"] = countryCode;
                var weatherData = await client.GetFromJsonAsync<OpenWeatherData>(url);
                if (weatherData != null)
                {
                    List<DateTime> dates = new List<DateTime>();
                    for(int i = 0; i < weatherData.list.Count; i++)
                    {
                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(weatherData.list[i].dt).DateTime;
                        if (!dates.Contains(date.Date))
                        {
                            dates.Add(date.Date);
                        }
                    }
                    return View(weatherData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> Stormglass()
        {
            var url = $"api/v2/Stormglass";
            try
            {
                var weatherData = await client.GetFromJsonAsync<StormglassData>(url);
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