using System.Text;
using Azure.AI.TextAnalytics;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace lab4
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly TextAnalyticsClient _textAnalyticsClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobSourceContainerClient;
        private readonly BlobContainerClient _blobDestinationContainerClient;

        public Function1(ILogger<Function1> logger, TextAnalyticsClient textAnalyticsClient, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _textAnalyticsClient = textAnalyticsClient;
            _blobServiceClient = blobServiceClient;

            _blobSourceContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("sourceContainerName"));
            _blobDestinationContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("targetContainerName"));
        }

        [Function(nameof(Function1))]
        public async Task Run([BlobTrigger("source/{name}", Source = BlobTriggerSource.LogsAndContainerScan, Connection = "blobConn")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob Trigger processed blob\n Name: {name} \n Data: {content}");


            var detectedLanguage = await _textAnalyticsClient.DetectLanguageAsync(content);
            var languageName = detectedLanguage.Value.Name;
            _logger.LogInformation($"Detected language: {languageName}");

            string targetBlobName = $"{languageName}/{name}";
            BlobClient blobClient = _blobDestinationContainerClient.GetBlobClient(targetBlobName);
            byte[] byteArray = Encoding.UTF8.GetBytes(content);

            await blobClient.UploadAsync(new MemoryStream(byteArray));
            _logger.LogInformation($"Uploaded blob - {targetBlobName} to {Environment.GetEnvironmentVariable("targetContainerName")} container.");

            await _blobSourceContainerClient.DeleteBlobIfExistsAsync(name);
            _logger.LogInformation($"Deleted blob - {name} from {Environment.GetEnvironmentVariable("sourceContainerName")} container.");
        }
    }
}
