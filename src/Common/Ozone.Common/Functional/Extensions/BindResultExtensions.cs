namespace Ozone.Common.Functional.Extensions;

public static class BindResultExtensions {
  public static Result<TResultValue> Bind<TSource, TResultValue>(
    this TSource result,
    Func<TSource, Result<TResultValue>> binder
  ) where TSource : IResult {
    if (result.IsFailure) {
      return Result.Failure<TResultValue>(result.Error);
    }

    return binder(result);
  }

  public static Result<TResultValue> Bind<TSource, TResultValue>(
    this TSource result,
    Func<Result<TResultValue>> binder
  ) where TSource : IResult {
    if (result.IsFailure) {
      return Result.Failure<TResultValue>(result.Error);
    }

    return binder();
  }

  public static async Task<Result<TResultValue>> Bind<TSource, TResultValue>(
    this Task<TSource> resultTask,
    Func<Result<TResultValue>> binder
  ) where TSource : IResult {
    var result = await resultTask;

    if (result.IsFailure) {
      return Result.Failure<TResultValue>(result.Error);
    }

    return binder();
  }

  public static Task<Result<TResultValue>> Bind<TSource, TResultValue>(
    this TSource result,
    Func<Task<Result<TResultValue>>> binder
  ) where TSource : IResult {
    if (result.IsFailure) {
      return Task.FromResult(Result.Failure<TResultValue>(result.Error));
    }

    return binder();
  }
}