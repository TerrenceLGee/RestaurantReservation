using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Users.Events;

namespace RestaurantReservation.Application.Features.Auth.Notifications;

public class RegistrationSuccessfulEventHandler(
    IEmailService emailService,
    ILogger<RegistrationSuccessfulEventHandler> logger) : INotificationHandler<RegistrationSuccessfulEvent>
{
    public async Task Handle(
        RegistrationSuccessfulEvent notification, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("New customer {FName} {LName} ({Email}) registered in the system",
            notification.FirstName,
            notification.LastName,
            notification.Email);

        var body = $"Your account has been successfully registered on {notification.RegistrationDate}. " +
                   $"Your username is {notification.Email}. " +
                   $"You are now able to schedule a reservation with any one of the restaurants in our system. " +
                   $"Thank you for joining our service. Have a great day!";
        var name = $"{notification.FirstName} {notification.LastName}";
        var subject = "Registration Successful!";

        var emailInfo = new EmailInfo(
            name,
            notification.Email,
            NotificationConstants.EmailSender,
            subject,
            body);

        await emailService.SendEmailAsync(emailInfo, cancellationToken);
    }
}