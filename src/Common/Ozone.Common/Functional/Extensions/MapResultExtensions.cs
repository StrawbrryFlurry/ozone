namespace Ozone.Common.Functional.Extensions;

public static class MapResultExtensions {
  public static Result<TResult> Map<TResult>(
    this Result result,
    Func<TResult> mapper
  ) {
    return result.IsFailure
      ? Result.Failure<TResult>(result.Error)
      : Result.Success(mapper());
  }

  public static Result<TResult> Map<TCurrent, TResult>(
    this Result<TCurrent> result,
    Func<TCurrent, TResult> mapper
  ) {
    return result.IsFailure
      ? Result.Failure<TResult>(result.Error)
      : Result.Success(mapper(result.Value));
  }

  public static async Task<Result<TResult>> Map<TCurrent, TResult>(
    this Task<Result<TCurrent>> resultTask,
    Func<TCurrent, TResult> func) {
    var result = await resultTask;

    if (result.IsFailure) {
      return Result.Failure<TResult>(result.Error);
    }

    var value = func(result.Value);

    return Result.Success(value);
  }

  public static async Task<Result<TResult>> Map<TResult>(this Task<Result> resultTask, Func<Task<TResult>> func) {
    var result = await resultTask;

    if (result.IsFailure) {
      return Result.Failure<TResult>(result.Error);
    }

    var value = await func();

    return Result.Success(value);
  }
}