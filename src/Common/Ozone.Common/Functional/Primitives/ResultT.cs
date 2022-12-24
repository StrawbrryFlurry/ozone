namespace Ozone.Common.Functional;

public readonly struct Result<TValue> : IResult<TValue> {
  public static readonly Result<TValue> Failed = Failure(Error.None);
  private readonly TValue? _value;

  public Result(TValue? value) {
    IsSuccess = true;
    Error = Error.None;
    _value = value;
  }

  public Result(Error error) {
    IsSuccess = false;
    Error = error;
  }

  public TValue Value => IsSuccess
    ? _value!
    : throw new InvalidOperationException("The value of a failure result can not be accessed.");

  public bool HasValue(out TValue value) {
    value = _value;
    return IsSuccess;
  }

  public static implicit operator Result<TValue>(TValue? value) {
    return Create(value);
  }

  private static Result<TValue> Create(TValue? value) {
    return Result.Create(value);
  }

  public static implicit operator Result<TValue>(Error error) {
    return Failure(error);
  }

  private static Result<TValue> Failure(Error error) {
    return new Result<TValue>(error);
  }

  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Error Error { get; }
}