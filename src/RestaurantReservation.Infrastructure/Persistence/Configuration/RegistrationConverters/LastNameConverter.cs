using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class LastNameConverter() 
    : ValueConverter<LastName, string>(ln => ln.Value, value => new LastName(value));