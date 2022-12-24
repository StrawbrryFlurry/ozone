using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Ozone.Common.Functional;

namespace Ozone.Testing.Common.FluentExtensions;

public static class ResultAssertions {
  public static void ShouldBeFailure(this IResult result) {
    Execute.Assertion
      .ForCondition(result.IsFailure)
      .FailWith("Expected result to have failed, but result has status `Successful`");
  }

  public static void ShouldFailWith(this IResult result, Error error) {
    ShouldBeFailure(result);

    Execute.Assertion
      .ForCondition(result.Error == error)
      .FailWith("Expected result to have error `{0}`, but result has error `{1}`", error, result.Error);
  }

  public static void ShouldBeSuccessful(this IResult result) {
    Execute.Assertion
      .ForCondition(result.IsSuccess)
      .FailWith("Expected result to be successful, but result failed with error `{0}`", result.Error);
  }

  /// <summary>
  /// Asserts that the result is successful and has a value that
  /// equals the expected value using its Object.Equals implementation.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="expectedValue"></param>
  /// <typeparam name="T"></typeparam>
  public static void ShouldSucceedWith<T>(this IResult<T> result, T expectedValue) {
    ShouldBeSuccessful(result);

    result.Value.Should().Be(expectedValue);
  }

  /// <summary>
  /// Asserts that the result is successful and it's value refers
  /// to the same reference as the expected value.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="expectedValue"></param>
  /// <typeparam name="T"></typeparam>
  public static void ShouldSucceedWithExactly<T>(this IResult<T> result, T expectedValue) {
    ShouldBeSuccessful(result);

    result.Value.Should().BeSameAs(expectedValue);
  }

  /// <summary>
  /// Asserts that the result is successful and it's values are equivalent to the expected value's.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="expectedValue"></param>
  /// <typeparam name="T"></typeparam>
  public static void ShouldSucceedWithEquivalent<T>(this IResult<T> result, T expectedValue) {
    ShouldBeSuccessful(result);
    result.Value.Should().BeEquivalentTo(expectedValue, (o) => o.RespectingRuntimeTypes());
  }
}