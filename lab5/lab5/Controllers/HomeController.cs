using Azure.AI.TextAnalytics;
using lab5.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace lab5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public HomeController(ILogger<HomeController> logger, TextAnalyticsClient textAnalyticsClient)
        {
            _logger = logger;
            _textAnalyticsClient = textAnalyticsClient;
        }

        public IActionResult Index()
        {
            return View(new TextInputModel());
        }

        [HttpPost]
        public IActionResult Analyze(TextInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                ModelState.AddModelError("", "Enter text.");
                return View("Index", model);
            }

            var response = _textAnalyticsClient.RecognizeLinkedEntities(model.Text);
            model.Entities = response.Value.Select(entity => new EntityModel
            {
                Name = entity.Name,
                Url = entity.Url?.ToString(),
                DataSource = entity.DataSource,
                Matches = entity.Matches.Select(m => new EntityMatch
                {
                    Text = m.Text,
                    ConfidenceScore = m.ConfidenceScore
                }).ToList()
            }).ToList();

            return View("Index", model);
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
