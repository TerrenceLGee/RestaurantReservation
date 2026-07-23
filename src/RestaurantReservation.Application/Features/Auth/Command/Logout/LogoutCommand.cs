using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Auth.Command.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;