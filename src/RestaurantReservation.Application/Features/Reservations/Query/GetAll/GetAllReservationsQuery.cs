using MediatR;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Reservations.Query.GetAll;

public record GetAllReservationsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? RestaurantName = null,
    string? Status = null) : IRequest<Result<PagedResult<ReservationResponse>>>;