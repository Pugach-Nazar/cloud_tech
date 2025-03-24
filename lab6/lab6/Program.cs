using Azure;
using Azure.AI.TextAnalytics;

var builder = WebApplication.CreateBuilder(args);

string languageKey = builder.Configuration["AzureAI:LanguageKey"];
string languageEndpoint = builder.Configuration["AzureAI:LanguageEndpoint"];

var credentials = new AzureKeyCredential(languageKey);
var endpoint = new Uri(languageEndpoint);

builder.Services.AddSingleton(new TextAnalyticsClient(endpoint, credentials));


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
