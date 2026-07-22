using System.Diagnostics.CodeAnalysis;

namespace RestaurantReservation.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public DomainError Error { get; }
    public List<DomainError> Errors { get; }

    protected Result(bool isSuccess, DomainError error)
    {
        switch (isSuccess)
        {
            case true when error != DomainError.None:
                throw new InvalidOperationException("Success result cannot have an error.");
            case false when error == DomainError.None:
                throw new InvalidOperationException("Failure result must have an error.");
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    protected Result(bool isSuccess, List<DomainError> errors)
    {
        switch (isSuccess)
        {
            case true when errors.Count != 0:
                throw new InvalidOperationException("Success result cannot have a collection of errors.");
            case false when errors.Count == 0:
                throw new InvalidOperationException("Failure must have at least one error in the collection.");
            default:
                IsSuccess = isSuccess;
                Errors = errors;
                break;
        }
    }

    public static Result Success() => new(true, DomainError.None);
    public static Result Failure(DomainError error) => new(false, error);
    public static Result Failure(List<DomainError> errors) => new(false, errors);
    public static Result<T> Success<T>(T value) => new(value, true, DomainError.None);
    public static Result<T> Failure<T>(DomainError error) => new(default, false, error);
    public static Result<T> Failure<T>(List<DomainError> errors) => new(default, false, errors);

    public static Result<T> Create<T>(T? value) => value is not null
        ? Success(value)
        : Failure<T>(DomainError.NullValue);
}

public class Result<T> : Result
{
    [NotNull]
    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");
    
    protected internal Result(T? value, bool isSuccess, DomainError error) : base(isSuccess, error)
    {
        Value = value;
    }

    protected internal Result(T? value, bool isSuccess, List<DomainError> errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T? value) => Create(value);
}