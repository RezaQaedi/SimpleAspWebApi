namespace BehvarTestProject.Services
{
    public interface IEmailSender
    {
        public Task<bool> SendAsync(string adress, string message, string title);
    }
}
