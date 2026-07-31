using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Tables.Errors;

public static class TableGroupErrors
{
    
    public static DomainError TableGroupNotFound(string groupName) => new(
        "TableGroup.NotFound",
        $"There was no table group with the name {groupName} found, cannot add table",
        ErrorType.NotFound);

    public static DomainError TableGroupNotFound() => new(
        "TableGroup.NotFound",
        "Table group not found",
        ErrorType.NotFound);
    
    public static DomainError TableGroupAlreadyExists(string groupName) => new(
        "TableGroup.AlreadyExists",
        $"Table group {groupName} already exists and cannot be added",
        ErrorType.Conflict);

    public static DomainError TableGroupNameAlreadyTaken(string groupName) => new(
        "TableGroup.NameAlreadyTaken",
        $"The table group name {groupName} is already taken, by a table group in this restaurant, unable to update group name",
        ErrorType.Conflict);
}