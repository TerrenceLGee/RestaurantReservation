using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Auth.Command.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;