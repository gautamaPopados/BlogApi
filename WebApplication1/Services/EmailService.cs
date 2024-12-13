using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using WebApplication1.Services.IServices;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using WebApplication1.Data.Entities;
using WebApplication1.Data;

namespace WebApplication1.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppDbContext _db;

        public EmailService(AppDbContext db)
        {
            _db = db;
        }
        public async Task SendPendingEmailsAsync()
        {
            var pendingEmails = await GetPendingEmailsAsync();
            foreach (var email in pendingEmails)
            {
                try
                {
                    await SendEmailAsync(email.Recipient, email.Subject, email.Body);
                    await MarkAsSentAsync(email);
                }
                catch (Exception)
                {
                    await IncrementRetryCountAsync(email);
                }
            }
        }

        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("localhost", 1025, SecureSocketOptions.None);
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(TextFormat.Plain) { Text = body }
            };
            message.From.Add(new MailboxAddress("Blog", "blog@blog.com"));
            message.To.Add(new MailboxAddress("Recipient", recipient));
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
        
        public async Task AddToQueueAsync(string recipient, string subject, string body)
        {
            var email = new EmailQueue
            {
                Recipient = recipient,
                Subject = subject,
                Body = body,
                IsSent = false
            };
            _db.EmailQueue.Add(email);
            await _db.SaveChangesAsync();
        }
        public async Task<List<EmailQueue>> GetPendingEmailsAsync(int maxRetry = 3)
        {
            return await _db.EmailQueue
                .Where(e => !e.IsSent && e.RetryCount < maxRetry)
                .ToListAsync();
        }

        public async Task MarkAsSentAsync(EmailQueue email)
        {
            email.IsSent = true;
            _db.EmailQueue.Update(email);
            await _db.SaveChangesAsync();
        }

        public async Task IncrementRetryCountAsync(EmailQueue email)
        {
            email.RetryCount++;
            _db.EmailQueue.Update(email);
            await _db.SaveChangesAsync();
        }
    }
}
