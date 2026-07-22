using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Common.Helpers;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Domain.Restaurants;

public class Restaurant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public ICollection<RestaurantSchedule> Schedule { get; set; } = [];
    public ICollection<RestaurantTable> Tables { get; set; } = [];
    public ICollection<TableGroup> TableGroups { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];
    
    private Restaurant() {}

    private Restaurant(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Restaurant Create(string name)
    {
        return new Restaurant(
            Guid.CreateVersion7(),
            name);
    }

    public void SetSchedule(List<RestaurantSchedule> schedule)
    {
        foreach (var day in schedule)
        {
            Schedule.Add(day);
        }
    }

    public Result Update(string? name, List<RestaurantSchedule>? schedule)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        if (schedule is not null)
        {
            foreach (var day in schedule)
            {
                var result = UpdateScheduleDay(day);
                if (result.IsFailure) return result;
            }
        }

        return Result.Success();
    }

    public Result UpdateScheduleDay(RestaurantSchedule scheduleDay)
    {
        if (!Enum.IsDefined(scheduleDay.Day))
        {
            return Result.Failure(RestaurantErrors.InvalidScheduleDay(scheduleDay.Day.ToString()));
        }

        if (!ValidationHelper.IsRestaurantScheduleStartAndEndTimeValid(scheduleDay.DailyHours[0], scheduleDay.DailyHours[1]))
        {
            return Result.Failure(RestaurantErrors.ScheduleStartComesAfterEnd);
        }

        var scheduleToUpdate = Schedule.FirstOrDefault(s => s.Day == scheduleDay.Day);

        if (scheduleToUpdate is null)
        {
            throw new InvalidOperationException(
                $"There was an unexpected error updating your schedule for {scheduleDay.Day}");
        }

        scheduleToUpdate = scheduleDay;

        return Result.Success();
    }

    public void AddTable(int seats)
    {
        var table = RestaurantTable.Create(Id, seats);
        Tables.Add(table);
    }

    public void AddTableGroup(string name, List<RestaurantTable> tables)
    {
        var tableGroup = TableGroup.Create(Id, name);
        tableGroup.AddTables(tables);
        TableGroups.Add(tableGroup);
    }
}