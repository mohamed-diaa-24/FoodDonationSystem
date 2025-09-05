namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userFirstName);
        Task<bool> SendEmailConfirmationAsync(string toEmail, string confirmationToken, string userFirstName);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userFirstName, string userRole);
        Task<bool> SendAccountApprovedEmailAsync(string toEmail, string userFirstName, string organizationType);
        Task<bool> SendAccountRejectedEmailAsync(string toEmail, string userFirstName, string organizationType, string reason);
    }
}
