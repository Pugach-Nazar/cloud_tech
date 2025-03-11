using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddAzureClients(b =>
        {
            b.AddBlobServiceClient(Environment.GetEnvironmentVariable("blobConn"));
            var endpoint = new Uri(Environment.GetEnvironmentVariable("textAnalyticsEndpoint"));
            var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("textAnalyticsKey"));
            b.AddTextAnalyticsClient(endpoint, credential);
        });
    })
    .Build();

host.Run();