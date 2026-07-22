using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class ReservationDateConverter() 
    : ValueConverter<ReservationDate, DateOnly>(r => r.Value, value => new ReservationDate(value));