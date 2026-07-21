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
}