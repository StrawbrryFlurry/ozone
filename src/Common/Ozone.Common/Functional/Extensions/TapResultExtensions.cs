namespace Ozone.Common.Functional.Extensions;

public static class TapResultExtensions {
  public static Result Tap(this Result result, Action action) {
    if (result.IsSuccess) {
      action();
    }

    return result;
  }

  public static Result<T> Tap<T>(this Result<T> result, Action action) {
    if (result.IsSuccess) {
      action();
    }

    return result;
  }


  public static Result<T> Tap<T>(this Result<T> result, Action<T> action) {
    if (result.IsSuccess) {
      action(result.Value);
    }

    return result;
  }

  public static async Task<Result<T>> Tap<T>(this Result<T> result, Func<Task> action) {
    if (result.IsSuccess) {
      await action();
    }

    return result;
  }

  public static async Task<Result<T>> Tap<T>(this Result<T> result, Func<T, Task> action) {
    if (result.IsSuccess) {
      await action(result.Value);
    }

    return result;
  }

  public static async Task<Result> Tap(this Task<Result> resultTask, Func<Task> action) {
    var result = await resultTask;

    if (result.IsSuccess) {
      await action();
    }

    return result;
  }

  public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Func<Task> action) {
    var result = await resultTask;

    if (result.IsSuccess) {
      await action();
    }

    return result;
  }

  public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Func<T, Task> action) {
    var result = await resultTask;

    if (result.IsSuccess) {
      await action(result.Value);
    }

    return result;
  }
}