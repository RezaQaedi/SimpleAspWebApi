namespace BehvarTestProject.Services
{
    public abstract class MessageSenderBase
    {
        public string Message { get; set; }

        protected MessageSenderBase(string message)
        {
            Message = message;
        }

        public abstract Task<bool> SendAsync();
    }
}
