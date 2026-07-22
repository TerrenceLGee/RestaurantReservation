using Microsoft.Extensions.Logging;

using MimeKit;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Notification;

namespace RestaurantReservation.Infrastructure.Email;

public class EmailService(ISmtpClientFactory smtpClientFactory, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Restaurant Reservations", emailInfo.From));
            msg.To.Add(new MailboxAddress($"{emailInfo.RecipientName}", $"{emailInfo.To}"));
            msg.Subject = $"{emailInfo.Subject}";
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = EmailHelper.GetFormattedEmailText(emailInfo.RecipientName, emailInfo.Body)
            };
            msg.Body = bodyBuilder.ToMessageBody();

            using var smtp = smtpClientFactory.Create();
            await smtp.ConnectAsync("localhost", 1025, false, cancellationToken);
            await smtp.SendAsync(msg, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("There was an unexpected error attempting to send an email via the email service class: {EX}", ex);
            throw;
        }
    }
}