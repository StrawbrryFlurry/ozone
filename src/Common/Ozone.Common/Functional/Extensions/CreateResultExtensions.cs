namespace Ozone.Common.Functional.Extensions;

public static class CreateResultExtensions {
  public static Result<TValue> ToResult<TValue>(this TValue? value) {
    return new Result<TValue>(value);
  }
}