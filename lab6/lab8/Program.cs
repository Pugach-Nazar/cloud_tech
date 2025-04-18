using Azure;
using Azure.AI.Translation.Text;


string key = "CloWbrBHe7JTJZtMgHFitWvIuxn9bOXsskDeNv6CHw5k7tKjxaVYJQQJ99BCACYeBjFXJ3w3AAAbACOG2jQT";

AzureKeyCredential credential = new(key);
TextTranslationClient client = new(credential, "eastus");

try
{
    string targetLanguage = "fr";
    string inputText = "This is a test.";

    Response<IReadOnlyList<TranslatedTextItem>> response = await client.TranslateAsync(targetLanguage, inputText).ConfigureAwait(false);
    IReadOnlyList<TranslatedTextItem> translations = response.Value;
    TranslatedTextItem translation = translations.FirstOrDefault();


    Console.WriteLine($"Detected languages of the input text: {translation?.DetectedLanguage?.Language} with score: {translation?.DetectedLanguage?.Confidence}.");
    Console.WriteLine($"Text was translated to: '{translation?.Translations?.FirstOrDefault().TargetLanguage}' and the result is: '{translation?.Translations?.FirstOrDefault()?.Text}'.");
}
catch (RequestFailedException exception)
{
    Console.WriteLine($"Error Code: {exception.ErrorCode}");
    Console.WriteLine($"Message: {exception.Message}");
}