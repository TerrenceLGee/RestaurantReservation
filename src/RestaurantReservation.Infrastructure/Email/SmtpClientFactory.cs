using MailKit.Net.Smtp;

using RestaurantReservation.Application.Abstractions;

namespace RestaurantReservation.Infrastructure.Email;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create()
    {
        return new SmtpClient();
    }
}