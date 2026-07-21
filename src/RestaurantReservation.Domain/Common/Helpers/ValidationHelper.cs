namespace RestaurantReservation.Domain.Common.Helpers;

public static class ValidationHelper
{
    public static bool IsStartAndEndTimeValid(TimeOnly? startTime, TimeOnly? endTime)
    {
        if (startTime.HasValue && endTime.HasValue)
        {
            return startTime < endTime;
        }

        return true;
    }
}