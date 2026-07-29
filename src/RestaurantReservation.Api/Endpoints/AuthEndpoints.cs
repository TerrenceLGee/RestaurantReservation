using MediatR;

using RestaurantReservation.Api.Extensions;
using RestaurantReservation.Application.Features.Auth.Command.Login;
using RestaurantReservation.Application.Features.Auth.Command.Logout;
using RestaurantReservation.Application.Features.Auth.Command.Register;
using RestaurantReservation.Domain.Users.Events;

namespace RestaurantReservation.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup(Constants.Auth.BaseUri)
            .WithTags(Constants.Auth.Tag);

        api.MapPost(Constants.Auth.Register, Register)
            .WithName("RegisterAccount")
            .WithSummary("Register a new user account");

        api.MapPost(Constants.Auth.Login, Login)
            .WithName("Login")
            .WithSummary("Login to your user account");

        api.MapPost(Constants.Auth.Logout, Logout)
            .WithName("Logout")
            .WithSummary("Logout from the system")
            .RequireAuthorization();
    }
    
    private static async Task<IResult> Register(
        RegisterUserCommand command,
        IMediator sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure) return result.ToProblemDetails();

        await sender.Publish(new RegistrationSuccessfulEvent(
            command.FirstName,
            command.LastName,
            command.EmailAddress,
            DateOnly.FromDateTime(DateTime.Now)), cancellationToken);

        return TypedResults.Ok("Registration successful!");
    }

    private static async Task<IResult> Login(
        LoginCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Logout(
        LogoutCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok("Logout successful")
            : result.ToProblemDetails();
    }
}