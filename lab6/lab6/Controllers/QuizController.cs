using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using lab6.Models;

namespace lab5.Controllers
{
    public class QuizController : Controller
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public QuizController(TextAnalyticsClient textAnalyticsClient)
        {
            _textAnalyticsClient = textAnalyticsClient;
        }

        public IActionResult Index()
        {
            return View(new QuizModel());
        }

        [HttpPost]
        public IActionResult GenerateQuiz(QuizModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                ModelState.AddModelError("", "Enter text.");
                return View("Index", model);
            }

            var response = _textAnalyticsClient.RecognizeEntities(model.Text);
            model.Questions = response.Value
                .Select(entity => new QuizQuestion
                {
                    Question = $"What does '{entity.Text}' mean?",
                    Answer = $"{entity.Category} ({entity.SubCategory})",
                    ConfidenceScore = entity.ConfidenceScore
                }).ToList();

            return View("Index", model);
        }
    }
}
