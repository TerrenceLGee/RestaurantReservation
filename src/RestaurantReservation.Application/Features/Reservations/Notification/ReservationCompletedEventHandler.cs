using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Reservations.Events;

namespace RestaurantReservation.Application.Features.Reservations.Notification;

public class ReservationCompletedEventHandler(
    IEmailService emailService,
    ILogger<ReservationCompletedEventHandler> logger) : INotificationHandler<ReservationCompletedEvent>
{
    public async Task Handle(
        ReservationCompletedEvent notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reservation {Id} scheduled for Customer {Email} " +
                              "on {Date} starting at {Start} and ending at {End} has been marked as completed",
            notification.ReservationId,
            notification.Email,
            notification.ReservationDate,
            notification.ReservationStartTime,
            notification.ReservationEndTime);

        var body = $"Your reservation at {notification.RestaurantName} on {notification.ReservationDate} from " +
                   $"{notification.ReservationStartTime} to {notification.ReservationEndTime} has been completed. We hope that you enjoyed your " +
                   $"time at {notification.RestaurantName} and will visit again soon!";
        var name = $"{notification.FirstName} {notification.LastName}";
        var subject = "Reservation Completed";

        var emailInfo = new EmailInfo(
            name,
            notification.Email,
            "restaurant-reservations@example.com",
            subject,
            body);

        await emailService.SendEmailAsync(emailInfo, cancellationToken);
    }
}