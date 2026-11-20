namespace DazzleJewels.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string recipientEmail);
    }
}