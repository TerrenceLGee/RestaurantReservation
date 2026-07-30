using MediatR;

using RestaurantReservation.Api.Extensions;
using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Command.Delete;
using RestaurantReservation.Application.Features.Restaurants.Command.Update;
using RestaurantReservation.Application.Features.Restaurants.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Api.Endpoints;

public static class RestaurantEndpoints
{
    public static void MapRestaurantEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Constants.Restaurant.BaseUri)
            .WithTags(Constants.Restaurant.Tag)
            .WithDescription("Available Restaurants");

        api.MapGet(Constants.Restaurant.GetAll, GetAll)
            .WithName("GetAllRestaurants")
            .WithSummary("Get a listing of all available restaurants");

        api.MapGet(Constants.Restaurant.Get, Get)
            .WithName("GetRestaurantById")
            .WithSummary("Get a restaurant by id");

        api.MapPost(Constants.Restaurant.Add, Add)
            .WithName("AddRestaurant")
            .WithSummary("Add a new restaurant to the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapPut(Constants.Restaurant.Update, Update)
            .WithName("UpdateRestaurant")
            .WithSummary("Update an existing restaurant in the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapDelete(Constants.Restaurant.Delete, Delete)
            .WithName("DeleteRestaurant")
            .WithSummary("Delete an existing restaurant that's in the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapPost(Constants.Restaurant.AddTable, AddTable)
            .WithName("AddTable")
            .WithSummary("Add a table to a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapPut(Constants.Restaurant.UpdateTable, UpdateTable)
            .WithName("UpdateTable")
            .WithSummary("Update a table in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));
    }

    private static async Task<IResult> GetAll(
        int? page,
        int? pageSize,
        string? sortBy,
        string? name,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAllRestaurantsQuery(
            page ?? 1,
            pageSize ?? 10,
            sortBy,
            name);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Get(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetRestaurantQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Add(
        AddRestaurantCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/restaurants/{result.Value.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateRestaurantQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRestaurantCommand(
            id,
            query.Name,
            query.Schedule);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Delete(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRestaurantCommand(id);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> AddTable(
        Guid id,
        AddTableQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddTableCommand(
            id,
            query.NumberOfSeats,
            query.TableGroup);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/restaurants/{id}/tables/{result.Value.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateTable(
        Guid restaurantId,
        Guid tableId,
        int? numberOfSeats,
        string? groupName,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTableCommand(
            restaurantId,
            tableId,
            numberOfSeats,
            groupName);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }
}