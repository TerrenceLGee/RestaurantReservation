using System.Linq.Dynamic.Core;
using System.Reflection;

using RestaurantReservation.Domain.Reservations;

namespace RestaurantReservation.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy) where T : class
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;

        var sortExpressions = new List<string>();

        foreach (var part in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) continue;

            var propertyPath = tokens[0];

            if (!IsValidPropertyPath(typeof(T), propertyPath)) continue;

            var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? "descending"
                : "ascending";
            
            sortExpressions.Add($"{propertyPath} {direction}");
        }

        return sortExpressions.Count > 0
            ? query.OrderBy(string.Join(", ", sortExpressions))
            : query;
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? search) where T : class
    {
        if (string.IsNullOrWhiteSpace(search)) return query;

        var searchTrimmed = search.Trim();

        var isDate = DateOnly.TryParse(searchTrimmed, out var parsedData);
        var isInt = int.TryParse(searchTrimmed, out var parsedInt);
        var isStatusEnum = Enum.TryParse<ReservationStatus>(searchTrimmed, out var parsedStatusEnum);

        var allProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var searchExpressions = new List<string>();
        var parameters = new List<object>();

        foreach (var property in allProperties)
        {
            if (property.PropertyType == typeof(string))
            {
                var paramIndex = parameters.Count;
                searchExpressions.Add($"{property.Name}.Contains(@{paramIndex})");
                parameters.Add(searchTrimmed);
            }
            else if (property.PropertyType == typeof(DateOnly) && isDate)
            {
                var paramIndex = parameters.Count;
                searchExpressions.Add($"{property.Name} == @{paramIndex}");
                parameters.Add(parsedData);
            }
            else if (property.PropertyType == typeof(int) && isInt)
            {
                var paramIndex = parameters.Count;
                searchExpressions.Add($"{property.Name} == @{paramIndex}");
                parameters.Add(parsedInt);
            }
            else if (property.PropertyType == typeof(ReservationStatus) && isStatusEnum)
            {
                var paramIndex = parameters.Count;
                searchExpressions.Add($"{property.Name} == @{paramIndex}");
                parameters.Add(parsedStatusEnum);
            }
        }

        if (searchExpressions.Count == 0) return query;

        var dynamicWhereClause = string.Join(" || ", searchExpressions);

        return query.Where(dynamicWhereClause, [.. parameters]);
    }
    
    private static bool IsValidPropertyPath(Type type, string path)
    {
        var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var currentType = type;

        foreach (var part in parts)
        {
            var prop = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name.Equals(part, StringComparison.OrdinalIgnoreCase));

            if (prop is null) return false;

            currentType = prop.PropertyType;
        }

        return true;
    }
}