using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Notification.Constants;
using RestaurantReservation.Domain.Reservations.Events;

namespace RestaurantReservation.Application.Features.Reservations.Notification;

public class ReservationScheduledEventHandler(
    IEmailService emailService,
    ILogger<ReservationScheduledEventHandler> logger) : INotificationHandler<ReservationScheduledEvent>
{
    public async Task Handle(
        ReservationScheduledEvent notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reservation has been scheduled for {Email} on {Date} starting at " +
                              "{Start} ending at {End}",
            notification.Email,
            notification.ReservationDate,
            notification.ReservationStartTime,
            notification.ReservationEndTime);

        var body = $"Your reservation has been scheduled at " +
                   $"{notification.RestaurantName} for {notification.ReservationDate} from {notification.ReservationStartTime} " +
                   $"to {notification.ReservationEndTime} with a party of {notification.NumberOfGuests} guests.";

        var name = $"{notification.FirstName} {notification.LastName}";
        var subject = "Reservation Scheduled";

        var emailInfo = new EmailInfo(
            name,
            notification.Email,
            NotificationConstants.EmailSender,
            subject,
            body);

        await emailService.SendEmailAsync(emailInfo, cancellationToken);
    }
}