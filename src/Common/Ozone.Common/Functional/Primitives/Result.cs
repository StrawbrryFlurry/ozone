using System.Runtime.CompilerServices;

namespace Ozone.Common.Functional;

public readonly struct Result : IResult {
  public static readonly Result Failed = Failure(Error.NullValue);

  public Result(Error? error) {
    if (error is null || Error.None.Equals(error)) {
      Error = default!;
      IsSuccess = true;
    }
    else {
      Error = error.Value;
      IsSuccess = false;
    }
  }

  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;

  public bool HasError(out Error error) {
    error = Error;
    return IsFailure;
  }

  public static bool HasFailure(out Error error, params IResult[] results) {
    foreach (var result in results) {
      if (!result.IsFailure) {
        continue;
      }

      error = result.Error;
      return true;
    }

    error = default;
    return false;
  }

  public Error Error { get; }

  public static implicit operator Result(Error error) {
    return Failure(error);
  }

  public static Result Success() {
    return new Result(Error.None);
  }

  public static Result<TValue> Success<TValue>(TValue value) {
    return new Result<TValue>(value);
  }

  public static Result Failure(Error error) {
    return new Result(error);
  }

  public static Result<TValue> Failure<TValue>(Error error) {
    return new Result<TValue>(error);
  }

  public static Result<TValue> Create<TValue>(TValue? value) {
    return value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
  }
}