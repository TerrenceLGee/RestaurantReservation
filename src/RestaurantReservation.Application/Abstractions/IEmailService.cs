using RestaurantReservation.Application.Features;

namespace RestaurantReservation.Application.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(EmailInfo emailInfo, CancellationToken cancellationToken = default);
}