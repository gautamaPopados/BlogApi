using Quartz;
using WebApplication1.Services.IServices;

namespace WebApplication1.Jobs
{
    public class EmailNotificationJob : IJob
    {
        private readonly IEmailService _emailService;

        public EmailNotificationJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //await _emailService.SendPendingEmailsAsync();
        }
    }
}
