using System.Runtime.InteropServices.JavaScript;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        result.CheckResult();

        var errorType = ErrorType.None;
        var description = string.Empty;
        var errorCode = string.Empty;
        
        if (result.Errors.Count > 0)
        {
            errorType = result.Errors[0].ErrorType;
            description = result.Errors[0].Description;
            errorCode = result.Errors[0].Code;
        }
        else
        {
            errorType = result.Error.ErrorType;
            description = result.Error.Description;
            errorCode = result.Error.Code;
        }

        (int statusCode, string title) = MapErrorType(errorType);

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: description,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = errorCode
            });
    }

    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        result.CheckResult();

        var errorType = ErrorType.None;
        var description = string.Empty;
        var errorCode = string.Empty;

        if (result.Errors.Count > 0)
        {
            errorType = result.Errors[0].ErrorType;
            description = result.Errors[0].Description;
            errorCode = result.Errors[0].Code;
        }
        else
        {
            errorType = result.Error.ErrorType;
            description = result.Error.Description;
            errorCode = result.Error.Code;
        }

        (int statusCode, string title) = MapErrorType(errorType);

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: description,
            extensions: new Dictionary<string, object?> { ["errorCode"] = errorCode });
    }
    
    
    private static (int StatusCode, string Title) MapErrorType(ErrorType type) => type switch
    {
        ErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation Error"),
        ErrorType.BadRequest => (StatusCodes.Status400BadRequest, "Bad Request"),
        ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
        ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
        ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
        ErrorType.CapacityExceeded => (StatusCodes.Status400BadRequest, "Bad Request: Capacity Exceeded"),
        _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
    };

    private static void CheckResult(this Result result)
    {
        if (result.IsSuccess || result.Error == DomainError.None)
        {
            throw new InvalidOperationException("Cannot convert a successful result to a problem detail");
        }
    }
}