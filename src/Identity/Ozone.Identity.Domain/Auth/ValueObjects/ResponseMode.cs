using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Errors;

namespace Ozone.Identity.Domain.Auth;

public sealed class ResponseMode : ValueObject {
  internal const string QueryValue = "query";
  internal const string FragmentValue = "fragment";

  public static readonly ResponseMode Query = new(QueryValue);
  public static readonly ResponseMode Fragment = new(FragmentValue);

  private ResponseMode(string value) {
    Value = value;
  }

  public string Value { get; private set; }

  public static Result<ResponseMode> CreateFrom(string value) {
    if (value is QueryValue or FragmentValue) {
      return new ResponseMode(value);
    }

    return DomainErrors.ValueObject.InvalidResponseMode;
  }

  protected override IEnumerable<object?> GetEqualityComponents() {
    yield return Value;
  }

  public override string ToString() {
    return Value;
  }
}