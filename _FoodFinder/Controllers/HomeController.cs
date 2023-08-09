using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FoodImageApp.Controllers
{
    public class HomeController : Controller
    {
        private const string apiKey = "3b4365a9ca034564a8a8c79414c304b8";
        private static readonly HttpClient client = new HttpClient();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string foodItem)
        {
            if (string.IsNullOrEmpty(foodItem))
            {
                return View();
            }

            string imageUrl = await FetchFoodImageAsync(foodItem);

            return View("Index", imageUrl);
        }

        private async Task<string> FetchFoodImageAsync(string query)
        {
            string apiUrl = $"https://api.spoonacular.com/food/search?query={query}&number=1&apiKey={apiKey}";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<FoodSearchResult>(responseStream, options);
                return result?.SearchResults[0]?.Results[0]?.Image;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class FoodSearchResult
    {
        public SearchResultCategory[] SearchResults { get; set; }
    }

    public class SearchResultCategory
    {
        public SearchResult[] Results { get; set; }
    }

    public class SearchResult
    {
        public string Image { get; set; }
    }
}
