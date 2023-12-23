using SendGrid;
using SendGrid.Helpers.Mail;

namespace BestStoreApi.Net_Core_7.Services
{
    public class EmailSender
    {
        private readonly string apiKey;
        private readonly string fromEmail;
        private readonly string senderName;
        
        public EmailSender(IConfiguration configuration)
        {
            apiKey = configuration["EmailSender:Apikey"];
            fromEmail = configuration["EmailSender:FromEmail"];
            senderName = configuration["EmailSender:SenderName"];
        }


        public async Task SendEmail(string subject, string toEmail, string username, string message)
        {
            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ilkinlikus@gmail.com", "Best Store");
            var to =  new EmailAddress(toEmail, username);
            var plainTextContent =message;
            var htmlContext = "<strng>and easy to anywhere , even with c#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContext);
            var response = await client.SendEmailAsync(msg);


        }
    }
}
