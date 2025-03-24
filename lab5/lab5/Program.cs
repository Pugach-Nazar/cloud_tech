using Azure;
using Azure.AI.TextAnalytics;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("AzureAI");

var credentials = new AzureKeyCredential(config["LanguageKey"]);
var endpoint = new Uri(config["LanguageEndpoint"]);
var textAnalyticsClient = new TextAnalyticsClient(endpoint, credentials);

builder.Services.AddSingleton(textAnalyticsClient);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
