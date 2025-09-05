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
            
            
            <a href='qoot://reset-password?token={resetToken}&email={Uri.EscapeDataString(toEmail)}' class='button'>
                Reset Password
            </a>
            
            <p><strong>Security Token:</strong> <code>{resetToken}</code></p>
            
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

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #4CAF50; color: white; padding: 20px; text-align: center;'>
            <h1>🍽️ Welcome to Food Donation System!</h1>
        </div>
        <div style='padding: 20px; background-color: #f9f9f9;'>
            <h2>Hello {userFirstName}!</h2>
            <p>Thank you for joining our mission to fight hunger and reduce food waste!</p>
            
            <p>Please confirm your email address by clicking the button below:</p>
            
            <a href='http://localhost:3000/confirm-email?token={confirmationToken}&email={Uri.EscapeDataString(toEmail)}' 
               style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0;'>
                Confirm Email Address
            </a>
            
            <p>If you didn't create this account, please ignore this email.</p>
        </div>
        <div style='text-align: center; padding: 20px; color: #666; font-size: 12px;'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userFirstName, string userRole)
        {
            var subject = "Welcome to Food Donation System!";
            var roleDescription = GetRoleDescription(userRole);

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #4CAF50; color: white; padding: 20px; text-align: center;'>
            <h1>🍽️ Welcome to Food Donation System!</h1>
        </div>
        <div style='padding: 20px; background-color: #f9f9f9;'>
            <h2>Hello {userFirstName}!</h2>
            <p>Welcome to the Food Donation System! We're excited to have you join our mission to reduce food waste and fight hunger.</p>
            
            <h3>Your Role: {userRole}</h3>
            <p>{roleDescription}</p>
            
            <h3>Next Steps:</h3>
            <ul>
                <li>Complete your profile information</li>
                <li>Upload required documents for verification</li>
                <li>Wait for admin approval (if applicable)</li>
                <li>Start making a difference in your community!</li>
            </ul>
            
            <p>If you have any questions, don't hesitate to contact our support team.</p>
        </div>
        <div style='text-align: center; padding: 20px; color: #666; font-size: 12px;'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendAccountApprovedEmailAsync(string toEmail, string userFirstName, string organizationType)
        {
            var subject = $"🎉 Your {organizationType} has been Approved!";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #4CAF50; color: white; padding: 20px; text-align: center;'>
            <h1>🎉 Congratulations!</h1>
        </div>
        <div style='padding: 20px; background-color: #f9f9f9;'>
            <h2>Hello {userFirstName}!</h2>
            <p>Great news! Your {organizationType.ToLower()} has been approved and is now active on the Food Donation System.</p>
            
            <p>You can now:</p>
            <ul>
                <li>Access all platform features</li>
                <li>Start donating or receiving food donations</li>
                <li>Connect with other organizations in your area</li>
                <li>Make a real difference in fighting hunger</li>
            </ul>
            
            <a href='http://localhost:3000/dashboard' 
               style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0;'>
                Go to Dashboard
            </a>
            
            <p>Thank you for joining our mission to reduce food waste and help those in need!</p>
        </div>
        <div style='text-align: center; padding: 20px; color: #666; font-size: 12px;'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }


        public async Task<bool> SendAccountRejectedEmailAsync(string toEmail, string userFirstName, string organizationType, string reason)
        {
            var subject = $"Update on Your {organizationType} Application";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #ff9800; color: white; padding: 20px; text-align: center;'>
            <h1>Application Update</h1>
        </div>
        <div style='padding: 20px; background-color: #f9f9f9;'>
            <h2>Hello {userFirstName}!</h2>
            <p>Thank you for your interest in joining the Food Donation System.</p>
            
            <p>After reviewing your {organizationType.ToLower()} application, we need additional information or documentation before we can approve your account.</p>
            
            <h3>Reason for Review:</h3>
            <p style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 10px; margin: 15px 0;'>
                {reason}
            </p>
            
            <p>Please:</p>
            <ul>
                <li>Review the feedback above</li>
                <li>Update your profile with the required information</li>
                <li>Upload any missing documents</li>
                <li>Contact our support team if you have questions</li>
            </ul>
            
            <a href='http://localhost:3000/profile' 
               style='background-color: #ff9800; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0;'>
                Update Profile
            </a>
            
            <p>We appreciate your patience and look forward to having you join our community!</p>
        </div>
        <div style='text-align: center; padding: 20px; color: #666; font-size: 12px;'>
            <p>© 2024 Food Donation System. Fighting hunger, one donation at a time.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        private static string GetRoleDescription(string role)
        {
            return role.ToLower() switch
            {
                "restaurant" => "As a restaurant partner, you can donate excess food to help reduce waste and feed those in need in your community.",
                "charity" => "As a charity organization, you can receive food donations to support the people you serve in your community.",
                "volunteer" => "As a volunteer, you can help deliver food donations from restaurants to charity organizations.",
                "individual" => "As an individual donor, you can donate excess food from your home or events to help those in need.",
                _ => "Welcome to our platform! You can now participate in our mission to reduce food waste and fight hunger."
            };
        }
    }
}
