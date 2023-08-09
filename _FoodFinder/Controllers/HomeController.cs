using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FoodImageApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            string apiKey = _configuration["AppSettings:ApiKey"];
            string apiUrl = $"https://api.spoonacular.com/food/search?query={query}&number=1&apiKey={apiKey}";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<FoodSearchResult>(responseContent, options);

                if (result?.SearchResults[0]?.Results.Length > 0)
                {
                    return result.SearchResults[0].Results[0].Image;
                }
                else
                {
                    // No results found, return a message
                    return "No image available for this query.";
                }
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
