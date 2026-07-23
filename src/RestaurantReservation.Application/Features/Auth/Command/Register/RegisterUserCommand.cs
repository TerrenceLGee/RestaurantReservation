using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Auth.Command.Register;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string EmailAddress,
    string PhoneNumber,
    string Password,
    string ConfirmPassword) : IRequest<Result>;