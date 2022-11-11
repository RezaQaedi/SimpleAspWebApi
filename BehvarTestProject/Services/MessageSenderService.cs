namespace BehvarTestProject.Services
{
    public class MessageSenderService : ISmsSneder, IEmailSender
    {
        public Task<bool> SendAsync(string adress, string message, string title)
        {
            var emailSender = new EmailSender(adress, message, title);
            return emailSender.SendAsync();
        }

        public Task<bool> SendAsync(string PhoneNumebr, string message)
        {
            var smsSender = new SmsSender(message, PhoneNumebr);
            return smsSender.SendAsync();
        }
    }
}
