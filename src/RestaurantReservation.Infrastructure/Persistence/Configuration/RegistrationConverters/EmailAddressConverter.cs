using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class EmailAddressConverter() 
    : ValueConverter<EmailAddress, string>(e => e.Value, value => new EmailAddress(value));