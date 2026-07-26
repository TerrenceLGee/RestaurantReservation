namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantScheduleResponse(string Day, TimeOnly?[] DailyHours);