using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Tables.Errors;

public static class TableErrors
{
    public static readonly DomainError TableCapacityExceeded = new(
        "Table.CapacityExceeded",
        "The number of guests in your party exceeds table capacity",
        ErrorType.CapacityExceeded);

    public static readonly DomainError TableAlreadyReserved = new(
        "Table.AlreadyReserved",
        "This table is already reserved for this time slot",
        ErrorType.Conflict);

    public static readonly DomainError TableCannotBeReserved = new(
        "Table.CannotBeReserved",
        "This table is not available for a reservation at this time",
        ErrorType.NotFound);

    public static readonly DomainError TableNotFreed = new(
        "Table.NotFreed",
        "Unable to free table(s) associated with this reservation",
        ErrorType.InternalServerError);

    public static readonly DomainError TableNotFound = new(
        "Table.NotFound",
        "Table not found",
        ErrorType.NotFound);

    public static readonly DomainError TablesNotFound = new(
        "Tables.NotFound",
        "Tables not found",
        ErrorType.NotFound);

    public static DomainError TableAlreadyInTableGroup(string groupName) => new(
        "Table.AlreadyInTableGroup",
        $"Table is already in table group {groupName}",
        ErrorType.Conflict);
}