using System.Security.Cryptography;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Errors;

namespace Ozone.Identity.Domain.Auth;

public sealed class CodeChallengeMode : ValueObject {
  private const string SHA256Value = "S256";
  private const string SHA512Value = "S512";

  public static readonly CodeChallengeMode SHA256 = new(SHA256Value);
  public static readonly CodeChallengeMode SHA512 = new(SHA512Value);

  private CodeChallengeMode(string value) {
    Value = value;
  }

  public string Value { get; private set; }

  public static Result<CodeChallengeMode> CreateFrom(string value) {
    if (value is SHA256Value or SHA512Value) {
      return new CodeChallengeMode(value);
    }

    return DomainErrors.ValueObject.InvalidCodeChallengeMode;
  }

  protected override IEnumerable<object?> GetEqualityComponents() {
    yield return Value;
  }

  public override string ToString() {
    return Value;
  }

  public HashAlgorithm GetHashAlgorithm() {
    return Value switch {
      SHA256Value => System.Security.Cryptography.SHA256.Create(),
      SHA512Value => System.Security.Cryptography.SHA512.Create()
    };
  }
}