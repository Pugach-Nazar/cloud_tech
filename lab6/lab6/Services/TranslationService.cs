using Azure;
using Azure.AI.Translation.Text;

namespace lab6.Services
{
    public class TranslationService
    {
        private readonly TextTranslationClient _client;

        public TranslationService(IConfiguration configuration)
        {
            string key = configuration["AzureTranslator:Key"];
            string region = configuration["AzureTranslator:Region"];
            _client = new TextTranslationClient(new AzureKeyCredential(key), region);
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            var response = await _client.TranslateAsync(targetLanguage, text);
            var translation = response.Value.FirstOrDefault();
            return translation?.Translations.FirstOrDefault()?.Text ?? "Translation failed.";
        }

        public async Task<Dictionary<string, string>> GetLanguagesAsync()
        {
            var response = await _client.GetSupportedLanguagesAsync();
            return response.Value.Translation.ToDictionary(l => l.Key, l => l.Value.Name);
        }

    }
}
