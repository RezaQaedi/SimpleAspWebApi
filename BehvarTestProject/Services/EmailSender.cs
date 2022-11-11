namespace BehvarTestProject.Services
{
    public class EmailSender : MessageSenderBase
    {
        private string _address;

        private string _title;

        public EmailSender(string message, string address, string title) : base(message)
        {
            _address = address;
            _title = title;
        }

        public override Task<bool> SendAsync()
        {
            return Task.FromResult(true);
        }
    }
}
