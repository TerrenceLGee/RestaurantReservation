namespace RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;

public record CustomerContactInfo(
    FirstName FirstName,
    LastName LastName,
    EmailAddress EmailAddress,
    TelephoneNumber TelephoneNumber);