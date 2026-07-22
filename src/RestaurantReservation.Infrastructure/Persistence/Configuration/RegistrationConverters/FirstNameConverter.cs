using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class FirstNameConverter() 
    : ValueConverter<FirstName, string>(fn => fn.Value, value => new FirstName(value));