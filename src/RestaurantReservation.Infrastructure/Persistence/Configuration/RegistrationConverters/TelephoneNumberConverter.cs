using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class TelephoneNumberConverter() 
    : ValueConverter<TelephoneNumber, string>(t => t.Value, value => new TelephoneNumber(value));