using MediatR;

using RestaurantReservation.Api.Extensions;
using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Command.Delete;
using RestaurantReservation.Application.Features.Restaurants.Command.Update;
using RestaurantReservation.Application.Features.Restaurants.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Delete;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Delete;
using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;
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

        api.MapDelete(Constants.Restaurant.DeleteTable, DeleteTable)
            .WithName("DeleteTable")
            .WithSummary("Delete a table that's in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Restaurant.GetTable, GetTable)
            .WithName("GetTable")
            .WithSummary("Get a table by id that's in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Restaurant.GetTables, GetTables)
            .WithName("GetTables")
            .WithSummary("Get all tables belonging to restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Restaurant.GetTablesByGroupName, GetTablesByGroupName)
            .WithName("GetTablesByGroupName")
            .WithSummary("Get all tables belonging to a table group in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapPost(Constants.Restaurant.AddTableGroup, AddTableGroup)
            .WithName("AddTableGroup")
            .WithSummary("Add a new table group to a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapPut(Constants.Restaurant.UpdateTableGroup, UpdateTableGroup)
            .WithName("UpdateTableGroup")
            .WithSummary("Update an existing table group in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapDelete(Constants.Restaurant.DeleteTableGroup, DeleteTableGroup)
            .WithName("DeleteTableGroup")
            .WithSummary("Delete an existing table group in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Restaurant.GetTableGroup, GetTableGroup)
            .WithName("GetTableGroup")
            .WithSummary("Get a table group that is in a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy => 
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Restaurant.GetTableGroups, GetTableGroups)
            .WithName("GetTableGroups")
            .WithSummary("Get all table groups belonging to a restaurant in the system. (Admins Only)")
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
        int? tablePage,
        int? tablePageSize,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetRestaurantQuery(
            id,
            tablePage ?? 1,
            tablePageSize ?? 10);
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
        UpdateTableQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTableCommand(
            restaurantId,
            tableId,
            query.NumberOfSeats,
            query.GroupName);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteTable(
        Guid restaurantId,
        Guid tableId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTableCommand(
            restaurantId,
            tableId);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetTable(
        Guid restaurantId,
        Guid tableId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetTableQuery(restaurantId, tableId);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetTables(
        Guid restaurantId,
        int? page,
        int? pageSize,
        string? sortBy,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTablesByRestaurantQuery(restaurantId, page ?? 1, pageSize ?? 10, sortBy);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetTablesByGroupName(
        Guid restaurantId,
        string tableGroupName,
        int? page,
        int? pageSize,
        string? sortBy,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTablesByTableGroupQuery(
            restaurantId,
            tableGroupName,
            page ?? 1,
            pageSize ?? 10,
            sortBy);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> AddTableGroup(
        Guid restaurantId,
        AddTableGroupQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddTableGroupCommand(
            restaurantId,
            query.Name,
            query.NumberOfTables,
            query.NumberOfSeats);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"{restaurantId}/tablegroups/{result.Value.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateTableGroup(
        Guid restaurantId,
        Guid tableGroupId,
        UpdateTableGroupQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTableGroupCommand(
            restaurantId,
            tableGroupId,
            query.GroupName,
            query.NumberOfTablesToAdd,
            query.NumberOfSeats);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteTableGroup(
        Guid restaurantId,
        Guid tableGroupId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTableGroupCommand(
            restaurantId,
            tableGroupId);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetTableGroup(
        Guid restaurantId,
        Guid tableGroupId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetTableGroupQuery(
            restaurantId,
            tableGroupId);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetTableGroups(
        Guid restaurantId,
        int? page,
        int? pageSize,
        string? sortBy,
        string? name,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetAllTableGroupsQuery(
            restaurantId,
            page ?? 1,
            pageSize ?? 10,
            sortBy,
            name);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
}