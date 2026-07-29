using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Reservations.Events;

namespace RestaurantReservation.Application.Features.Reservations.Notification;

public class ReservationCanceledEventHandler(
    IEmailService emailService,
    ILogger<ReservationCanceledEventHandler> logger) : INotificationHandler<ReservationCanceledEvent>
{
    public async Task Handle(
        ReservationCanceledEvent notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reservation {Id} scheduled for Customer {Email} " +
                              "on {Date} starting at {Start} and ending at {End} has been canceled on {CancellationDate}",
            notification.ReservationId,
            notification.Email,
            notification.ReservationDate,
            notification.ReservationStartTime,
            notification.ReservationEndTime,
            notification.ReservationCanceledAtUtc);

        var body = $"Your reservation at {notification.RestaurantName} on {notification.ReservationDate} from " +
                   $"{notification.ReservationStartTime} to {notification.ReservationEndTime} has been canceled as per your request " +
                   $"on {notification.ReservationCanceledAtUtc}. No further action is necessary on your part.";
        var name = $"{notification.FirstName} {notification.LastName}";
        var subject = "Reservation Canceled";

        var emailInfo = new EmailInfo(
            name,
            notification.Email,
            NotificationConstants.EmailSender,
            subject,
            body);

        await emailService.SendEmailAsync(emailInfo, cancellationToken);
    }
}