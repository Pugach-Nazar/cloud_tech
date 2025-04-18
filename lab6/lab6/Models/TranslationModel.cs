namespace lab6.Models
{
    public class TranslationModel
    {
        public string Text { get; set; }
        public string TranslatedText { get; set; }
        public string SelectedLanguage { get; set; }
        public Dictionary<string, string> AvailableLanguages { get; set; }
    }
}
