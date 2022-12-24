namespace Ozone.Common.Functional.Extensions;

public static class ResultFunctionalExtensions {
  public static Result Ensure(this Result result, Func<bool> predicate) {
    return result.IsFailure || predicate()
      ? Result.Failed
      : result;
  }

  public static Result<TValue> Ensure<TValue>(
    this Result<TValue> result,
    Func<TValue, bool> predicate
  ) {
    return result.IsFailure || !predicate(result.Value)
      ? Result<TValue>.Failed
      : result;
  }

  public static Result OnFailureReturnError(
    this Result result,
    Error error
  ) {
    return result.IsFailure
      ? Result.Failure(error)
      : result;
  }

  public static Result<TValue> OnFailureReturnError<TValue>(
    this Result<TValue> result,
    Error error
  ) {
    return result.IsFailure
      ? Result.Failure<TValue>(error)
      : result;
  }

  public static Result<TValue> OnFailureCompensate<TValue>(
    this Result<TValue> result,
    TValue value
  ) {
    return result.IsFailure
      ? Result.Success(value)
      : result;
  }

  public static Result<TResult> TryMap<TValue, TResult>(
    this Result<TValue> result,
    Func<TValue, TResult> mapper,
    Func<Exception, Error> errorHandler
  ) {
    if (result.IsFailure) {
      return Result.Failure<TResult>(result.Error);
    }

    try {
      return Result.Success(mapper(result.Value));
    }
    catch (Exception e) {
      return Result.Failure<TResult>(errorHandler(e));
    }
  }

  public static Result<TResult> MergeMap<TResult, TValue1, TValue2>(
    this Result<(Result<TValue1>, Result<TValue2>)> result,
    Func<TValue1, TValue2, TResult> mapper
  ) {
    if (result.IsFailure) {
      return Result.Failure<TResult>(result.Error);
    }

    var (result1, result2) = result.Value;
    return Result.Success(mapper(result1.Value, result2.Value));
  }

  public static Result<TResult> MergeMap<TResult, TValue1, TValue2, TValue3>(
    this Result<(Result<TValue1>, Result<TValue2>, Result<TValue3>)> result,
    Func<TValue1, TValue2, TValue3, TResult> mapper
  ) {
    if (result.IsFailure) {
      return Result.Failure<TResult>(result.Error);
    }

    var (result1, result2, result3) = result.Value;
    return Result.Success(mapper(result1.Value, result2.Value, result3.Value));
  }

  public static Result<(TResult1 Result1, TResult2 Result2)> MapToMany<TSourceValue, TResult1, TResult2>(
    this Result<TSourceValue> source,
    Func<TSourceValue, TResult1> result1Factory,
    Func<TSourceValue, TResult2> result2Factory
  )
    where TResult1 : IResult
    where TResult2 : IResult {
    if (source.IsFailure) {
      return Result.Failure<(TResult1, TResult2)>(source.Error);
    }

    var sourceValue = source.Value;

    var result1 = result1Factory(sourceValue);
    if (result1.IsFailure) {
      return Result.Failure<(TResult1, TResult2)>(result1.Error);
    }

    var result2 = result2Factory(sourceValue);
    if (result2.IsFailure) {
      return Result.Failure<(TResult1, TResult2)>(result2.Error);
    }

    return Result.Success((result1, result2));
  }

  public static Result<(TResult1 Result1, TResult2 Result2, TResult3 Result3)> MapToMany<TSourceValue, TResult1,
    TResult2, TResult3>(
    this Result<TSourceValue> source,
    Func<TSourceValue, TResult1> result1Factory,
    Func<TSourceValue, TResult2> result2Factory,
    Func<TSourceValue, TResult3> result3Factory
  )
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult {
    if (source.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3)>(source.Error);
    }

    var sourceValue = source.Value;

    var result1 = result1Factory(sourceValue);
    if (result1.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3)>(result1.Error);
    }

    var result2 = result2Factory(sourceValue);
    if (result2.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3)>(result2.Error);
    }

    var result3 = result3Factory(sourceValue);
    if (result3.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3)>(result3.Error);
    }

    return Result.Success((result1, result2, result3));
  }

  public static Result<(TResult1 Result1, TResult2 Result2, TResult3 Result3, TResult4 Result4)> MapToMany<
    TSourceValue, TResult1, TResult2, TResult3, TResult4>(
    this Result<TSourceValue> source,
    Func<TSourceValue, TResult1> result1Factory,
    Func<TSourceValue, TResult2> result2Factory,
    Func<TSourceValue, TResult3> result3Factory,
    Func<TSourceValue, TResult4> result4Factory
  )
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult
    where TResult4 : IResult {
    if (source.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3, TResult4)>(source.Error);
    }

    var sourceValue = source.Value;

    var result1 = result1Factory(sourceValue);
    if (result1.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3, TResult4)>(result1.Error);
    }

    var result2 = result2Factory(sourceValue);
    if (result2.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3, TResult4)>(result2.Error);
    }

    var result3 = result3Factory(sourceValue);
    if (result3.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3, TResult4)>(result3.Error);
    }

    var result4 = result4Factory(sourceValue);
    if (result4.IsFailure) {
      return Result.Failure<(TResult1, TResult2, TResult3, TResult4)>(result4.Error);
    }

    return Result.Success((result1, result2, result3, result4));
  }

  public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action) {
    if (result.IsSuccess) {
      action(result.Value);
    }

    return result;
  }

  public static bool HasError<TResult>(this IEnumerable<TResult> results, out Error error) where TResult : IResult {
    error = default;
    foreach (var result in results) {
      if (!result.IsFailure) {
        continue;
      }

      error = result.Error;
      return true;
    }

    return false;
  }

  public static IEnumerable<TValue> GetValues<TValue>(this IEnumerable<Result<TValue>> results) {
    foreach (var result in results) {
      yield return result.Value;
    }
  }
}