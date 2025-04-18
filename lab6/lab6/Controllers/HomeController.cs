using System.Diagnostics;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace lab6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string TenantId;     // Azure subscription TenantId
        private readonly string ClientId;     // Microsoft Entra ApplicationId
        private readonly string ClientSecret; // Microsoft Entra Application Service Principal password
        private readonly string Subdomain;    // Immersive Reader resource subdomain

        private IConfidentialClientApplication _confidentialClientApplication;
        private IConfidentialClientApplication ConfidentialClientApplication
        {
            get
            {
                if (_confidentialClientApplication == null)
                {
                    _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(ClientId)
                    .WithClientSecret(ClientSecret)
                    .WithAuthority($"https://login.windows.net/{TenantId}")
                    .Build();
                }

                return _confidentialClientApplication;
            }
        }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            TenantId = configuration["TenantId"];
            ClientId = configuration["ClientId"];
            ClientSecret = configuration["ClientSecret"];
            Subdomain = configuration["Subdomain"];
        }

        /// <summary>
        /// Get a Microsoft Entra ID authentication token
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            const string resource = "https://cognitiveservices.azure.com/";

            var authResult = await ConfidentialClientApplication.AcquireTokenForClient(
                new[] { $"{resource}/.default" })
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authResult.AccessToken;
        }

        [HttpGet]
        public async Task<JsonResult> GetTokenAndSubdomain()
        {
            try
            {
                string tokenResult = await GetTokenAsync();

                return new JsonResult(new { token = tokenResult, subdomain = Subdomain });
            }
            catch (Exception e)
            {
                string message = "Unable to acquire Microsoft Entra token. Check the console for more information.";
                Debug.WriteLine(message, e);
                return new JsonResult(new { error = message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
