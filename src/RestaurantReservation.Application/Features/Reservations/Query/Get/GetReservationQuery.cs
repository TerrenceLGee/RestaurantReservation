using MediatR;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Reservations.Query.Get;

public record GetReservationQuery(Guid ReservationId) : IRequest<Result<ReservationDetailResponse>>;