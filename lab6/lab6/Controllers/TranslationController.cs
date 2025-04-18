using lab6.Models;
using lab6.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    public class TranslationController : Controller
    {
        private readonly TranslationService _translationService;

        public TranslationController(TranslationService translationService)
        {
            _translationService = translationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new TranslationModel
            {
                AvailableLanguages = await _translationService.GetLanguagesAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Translate(TranslationModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text) || string.IsNullOrWhiteSpace(model.SelectedLanguage))
            {
                ModelState.AddModelError("", "Please enter text and select a target language.");
                model.AvailableLanguages = await _translationService.GetLanguagesAsync();
                return View("Index", model);
            }

            model.TranslatedText = await _translationService.TranslateTextAsync(model.Text, model.SelectedLanguage);
            model.AvailableLanguages = await _translationService.GetLanguagesAsync();

            return View("Index", model);
        }
    }
}
