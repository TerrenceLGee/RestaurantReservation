using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Notification.Constants;
using RestaurantReservation.Domain.Reservations.Events;

namespace RestaurantReservation.Application.Features.Reservations.Notification;

public class ReservationRescheduledEventHandler(
    IEmailService emailService,
    ILogger<ReservationRescheduledEventHandler> logger) : INotificationHandler<ReservationRescheduledEvent>
{
    public async Task Handle(
        ReservationRescheduledEvent notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reservation has been rescheduled for {Email} date has been changed from {OriginalDate} to " +
                              "{NewDate} and from start and end time of {OriginalStart} to {OriginalEnd} to new start and end time " +
                              "of {NewStart} to {NewEnd}",
            notification.Email,
            notification.OriginalReservationDate,
            notification.ReservationDate,
            notification.OriginalReservationStartTime,
            notification.OriginalReservationEndTime,
            notification.ReservationStartTime,
            notification.ReservationEndTime);

        var body = $"Your reservation originally scheduled at {notification.RestaurantName} for {notification.OriginalReservationDate} from " +
                   $"{notification.OriginalReservationStartTime} to {notification.OriginalReservationEndTime} has been rescheduled for " +
                   $"{notification.ReservationDate} from {notification.ReservationStartTime} to {notification.ReservationEndTime} with a " +
                   $"party of {notification.NumberOfGuests} guests.";

        var name = $"{notification.FirstName} {notification.LastName}";
        var subject = "Reservation Rescheduled";

        var emailInfo = new EmailInfo(
            name,
            notification.Email,
            NotificationConstants.EmailSender,
            subject,
            body);

        await emailService.SendEmailAsync(emailInfo, cancellationToken);
    }
}