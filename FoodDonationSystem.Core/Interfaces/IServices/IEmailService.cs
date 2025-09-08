namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userFirstName);
        Task<bool> SendEmailConfirmationAsync(string toEmail, string confirmationToken, string userFirstName);
    }
}
