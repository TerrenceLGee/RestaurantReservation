using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class GuestsInPartyConverter() 
    : ValueConverter<GuestsInParty, int>(g => g.Value, value => new GuestsInParty(value));