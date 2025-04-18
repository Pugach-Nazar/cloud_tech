using Azure;
using Azure.AI.TextAnalytics;
using lab6.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.Face;

var builder = WebApplication.CreateBuilder(args);

string languageKey = builder.Configuration["AzureAI:LanguageKey"];
string languageEndpoint = builder.Configuration["AzureAI:LanguageEndpoint"];


var credentials = new AzureKeyCredential(languageKey);
var endpoint = new Uri(languageEndpoint);

builder.Services.AddSingleton(new TextAnalyticsClient(endpoint, credentials));
builder.Services.AddSingleton<TranslationService>();


builder.Services.AddSingleton(new ComputerVisionClient(
    new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(builder.Configuration["ComputerVision:Key"]))
{ Endpoint = builder.Configuration["ComputerVision:Endpoint"] });

builder.Services.AddSingleton(new FaceClient(
    new Microsoft.Azure.CognitiveServices.Vision.Face.ApiKeyServiceClientCredentials(builder.Configuration["FaceApi:Key"]))
{ Endpoint = builder.Configuration["FaceApi:Endpoint"] });


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Cookie.SameSite = SameSiteMode.Lax;

});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Configuration.AddUserSecrets<Program>();

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
