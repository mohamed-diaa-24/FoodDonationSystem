using Microsoft.AspNetCore.Mvc;

namespace FoodDonationSystem.API.Controllers
{
    [Route("api/open-app")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OpenAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OpenAppController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token, string email)
        {
            var deepLink = $"qoot://reset-password?token={token}&email={email}";
            var fallbackUrl = $"https://play.google.com/store/apps?hl=ar";

            var html = $@"
        <html>
        <head>
            <title>Opening App...</title>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <script>
                window.onload = function() {{
                    window.location = '{deepLink}';
                    setTimeout(function() {{
                        window.location = '{fallbackUrl}';
                    }}, 2000);
                }};
            </script>
        </head>
        <body>
            <p>Opening the app, please wait...</p>
        </body>
        </html>";

            return Content(html, "text/html");
        }

        [HttpGet("confirm-email")]
        public IActionResult ConfirmEmail(string token, string email)
        {
            var deepLink = $"qoot://ConfirmEmail?token={token}&email={email}";
            var fallbackUrl = $"https://play.google.com/store/apps?hl=ar";

            var html = $@"
        <html>
        <head>
            <title>Opening App...</title>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <script>
                window.onload = function() {{
                    window.location = '{deepLink}';
                    setTimeout(function() {{
                        window.location = '{fallbackUrl}';
                    }}, 2000);
                }};
            </script>
        </head>
        <body>
            <p>Opening the app, please wait...</p>
        </body>
        </html>";

            return Content(html, "text/html");
        }
    }
}
