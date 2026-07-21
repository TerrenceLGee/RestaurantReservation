namespace RestaurantReservation.Domain.Restaurants;

public record RestaurantSchedule(WeekDay Day, TimeOnly?[] DailyHours);