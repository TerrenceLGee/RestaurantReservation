using RestaurantReservation.Application.Features.Reservations.Notification;

namespace RestaurantReservation.Application.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default);
}