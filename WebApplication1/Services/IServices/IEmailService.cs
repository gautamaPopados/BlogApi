namespace WebApplication1.Services.IServices
{
    public interface IEmailService
    {
        public Task SendPendingEmailsAsync();

        public Task SendEmailAsync(string recipient, string subject, string body);

    }
}
