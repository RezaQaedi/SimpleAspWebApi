namespace BehvarTestProject.Services
{
    public interface ISmsSneder
    {
        public Task<bool> SendAsync(string PhoneNumebr, string message);
    }
}
