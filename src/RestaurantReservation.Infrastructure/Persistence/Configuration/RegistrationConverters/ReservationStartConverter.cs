using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class ReservationStartConverter() 
    : ValueConverter<ReservationStart, TimeOnly>(r => r.Value, value => new ReservationStart(value));