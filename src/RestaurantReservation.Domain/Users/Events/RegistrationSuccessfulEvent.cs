using MediatR;

namespace RestaurantReservation.Domain.Users.Events;

public record RegistrationSuccessfulEvent(
    string FirstName,
    string LastName,
    string Email,
    DateOnly RegistrationDate) : INotification;