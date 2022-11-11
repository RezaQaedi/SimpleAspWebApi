namespace BehvarTestProject.Services
{
    public class SmsSender : MessageSenderBase
    {
        private string _phoneNumber;

        public SmsSender(string message, string phoneNumber) : base(message) 
        {
            _phoneNumber = phoneNumber;
        }

        public override Task<bool> SendAsync()
        {
            return Task.FromResult(true);
        }
    }
}
