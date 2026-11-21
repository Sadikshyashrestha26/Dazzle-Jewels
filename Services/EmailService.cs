using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MailKit;

namespace DazzleJewels.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmailAsync(string recipientEmail)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]));

            message.To.Add(MailboxAddress.Parse(recipientEmail));

            message.Subject = "Welcome to Dazzle Jewels!";

            message.Body = new BodyBuilder
            {
                HtmlBody = @"
                    <h2>Welcome to Dazzle Jewels! ✨</h2>
                    <p>Thank you for subscribing to our newsletter.</p>
                    <p>Stay tuned for beautiful new collections,
                    exclusive offers and special updates!</p>
                    <p>With love,<br/>Team Dazzle Jewels</p>"
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                emailSettings["Host"],
                int.Parse(emailSettings["Port"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                emailSettings["Username"],
                emailSettings["Password"]);

            await client.SendAsync(new SendRequest(message));

            await client.DisconnectAsync(true);
        }
    }
}