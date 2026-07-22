using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class ReservationEndConverter() 
    : ValueConverter<ReservationEnd, TimeOnly>(r => r.Value, value => new ReservationEnd(value));