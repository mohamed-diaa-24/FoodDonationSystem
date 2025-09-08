using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FoodDonationSystem.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var emailSettings = _configuration.GetSection("EmailSettings");
            _smtpHost = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            _smtpUsername = emailSettings["SmtpUsername"] ?? "";
            _smtpPassword = emailSettings["SmtpPassword"] ?? "";
            _fromEmail = emailSettings["FromEmail"] ?? "";
            _fromName = emailSettings["FromName"] ?? "Food Donation System";
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mailMessage.To.Add(toEmail);

                if (!string.IsNullOrEmpty(plainTextBody))
                {
                    var plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, Encoding.UTF8, "text/plain");
                    var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");

                    mailMessage.AlternateViews.Add(plainView);
                    mailMessage.AlternateViews.Add(htmlView);
                }

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userFirstName)
        {
            var subject = "Reset Your Password - Qoot App";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🍽️ Qoot App</h1>
        </div>
        <div class='content'>
         <!--   <h2>Hello {userFirstName}!</h2> -->
            <p>We received a request to reset your password. If you made this request, click the button below to reset your password:</p>
            
            
            <a href='{resetToken}' class='button'>
                Reset Password
            </a>
            
            <p>If you didn't request this password reset, please ignore this email. Your password will remain unchanged.</p>
            
            <p><strong>Important:</strong> This link will expire in 1 hour for security reasons.</p>
        </div>
        <div class='footer'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>";

            var plainTextBody = $@"
Hello {userFirstName}!

We received a request to reset your password for your Food Donation System account.

Security Token: {resetToken}

If you didn't request this password reset, please ignore this email.

This token will expire in 1 hour for security reasons.

© 2024 Food Donation System
";

            return await SendEmailAsync(toEmail, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendEmailConfirmationAsync(string toEmail, string confirmationToken, string userFirstName)
        {
            var subject = "Confirm Your Email - Food Donation System";

            var htmlBody = @$"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🍽️ Qoot App</h1>
        </div>
        <div class='content'>
         <!--   <h2>Hello {userFirstName}!</h2> -->
                        <p>Please confirm your email address by clicking the button below:</p>

            
            <a href='{confirmationToken}' class='button'>
                confirm email
            </a>
            
                        <p>If you didn't create this account, please ignore this email.</p>
            
            <p><strong>Important:</strong> This link will expire in 1 hour for security reasons.</p>
        </div>
        <div class='footer'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>
";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}
