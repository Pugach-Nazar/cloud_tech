using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using lab6.Models;
using lab6.Services;

namespace lab5.Controllers
{
    public class QuizController : Controller
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;
        private readonly TranslationService _translationService;

        public QuizController(TextAnalyticsClient textAnalyticsClient, TranslationService translationService)
        {
            _textAnalyticsClient = textAnalyticsClient;
            _translationService = translationService;
        }

        public IActionResult Index()
        {
            return View(new QuizModel());
        }

        [HttpPost]
        public async Task<IActionResult> GenerateQuiz(QuizModel model)
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

            model.TranslatedText = await _translationService.TranslateTextAsync(model.Text, "uk");
            return View("Index", model);
        }
    }
}
