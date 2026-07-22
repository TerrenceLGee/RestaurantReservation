using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Restaurants.Errors;

public static class RestaurantErrors
{
    public static DomainError InvalidScheduleDay(string name) => new(
        "Restaurant.InvalidScheduleDay",
        "The day that you are trying to schedule is unknown",
        ErrorType.Validation);

    public static DomainError ScheduleStartComesAfterEnd => new(
        "Restaurant.InvalidScheduleTimes",
        "When setting scheduling the opening time must always come before the closing time",
        ErrorType.Validation);

    public static DomainError RestaurantIsAtFullCapacity(string name) => new(
        "Restaurant.AtFullCapacity",
        $"{name} is at full capacity at the time that you wish to schedule your reservation. Unable to reserve a table for this time",
        ErrorType.CapacityExceeded);

    public static DomainError RestaurantScheduleIsIncomplete(string name) => new(
        "Restaurant.ScheduleIsIncomplete",
        $"The schedule for {name} is incomplete, even if you are not open 7 days a week, you must have a schedule for each day",
        ErrorType.BadRequest);

    public static DomainError RestaurantScheduleIsInvalid => new(
        "Restaurant.ScheduleIsInvalid",
        "The schedule is invalid. Each day's opening time must come before the closing time",
        ErrorType.BadRequest);

    public static DomainError RestaurantClosedToday(string name, DateOnly date) => new(
        "Restaurant.Closed",
        $"{name} is closed on {date}, unable to make a reservation",
        ErrorType.BadRequest);

    public static DomainError RestaurantInvalidDay(string name, DateOnly date) => new(
        "Restaurant.InvalidDay",
        $"The date {date} is invalid to schedule a reservation at {name}",
        ErrorType.BadRequest);

    public static DomainError HoursOutOfRange(string name, DateOnly date, TimeOnly start, TimeOnly end) => new(
        "Restaurant.InvalidDay",
        $"The date {date} is invalid to schedule a reservation at {name}",
        ErrorType.BadRequest);

    public static DomainError NotFound(string name) => new(
        "Restaurant.NotFound",
        $"{name} was not found",
        ErrorType.NotFound);

    public static DomainError NotFound() => new(
        "Restaurant.NotFound",
        "Restaurant not found",
        ErrorType.NotFound);

    public static DomainError AlreadyExists(string name) => new(
        "Restaurant.AlreadyExistsInSystem",
        $"{name} is already in the system and cannot be added",
        ErrorType.Conflict);
}