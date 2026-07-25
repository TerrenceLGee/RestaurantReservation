using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Reservations.Events;

namespace RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

public class ReservationCanceledEventHandler(
    IEmailService emailService,
    ILogger<ReservationCanceledEventHandler> logger) : INotificationHandler<ReservationCanceledEvent>
{
    public async Task Handle(
        ReservationCanceledEvent notification, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}