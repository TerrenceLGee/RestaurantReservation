using MailKit.Net.Smtp;

namespace RestaurantReservation.Application.Abstractions;

public interface ISmtpClientFactory
{
    ISmtpClient Create();
}