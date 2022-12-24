using System.Security.Cryptography;
using Newtonsoft.Json;
using Ozone.Common.Functional;

// ReSharper disable InconsistentNaming

namespace Ozone.Identity.Core.Security.Signing;

[JsonConverter(typeof(SigningAlgorithmJsonConverter))]
public struct SigningAlgorithm : IEquatable<SigningAlgorithm> {
  internal const string RS256Value = "RS256";
  internal const string RS384Value = "RS384";
  internal const string RS512Value = "RS512";
  internal const string ES256Value = "ES256";
  internal const string ES384Value = "ES384";
  internal const string ES512Value = "ES512";

  private readonly string _name;

  public SigningAlgorithm(string name) {
    _name = name;
  }

  public static SigningAlgorithm Ec256 = new(ES256Value);
  public static SigningAlgorithm Ec384 = new(ES384Value);
  public static SigningAlgorithm Ec512 = new(ES512Value);

  public static SigningAlgorithm RS256 = new(RS256Value);
  public static SigningAlgorithm RS384 = new(RS384Value);
  public static SigningAlgorithm RS512 = new(RS512Value);

  public static SigningAlgorithm Default = RS256;

  public static Result<SigningAlgorithm> Parse(string algorithm) {
    return algorithm switch {
      ES256Value => Ec256,
      ES384Value => Ec384,
      ES512Value => Ec512,
      RS256Value => RS256,
      RS384Value => RS384,
      RS512Value => RS512,
      _ => Error.ArgumentOutOfRange(algorithm)
    };
  }

  public static Result<SigningAlgorithm> Parse(AsymmetricAlgorithm algorithm) {
    if (algorithm is RSA rsa) {
      return ParseRsa(rsa);
    }

    if (algorithm is ECDsa ec) {
      return ParseEc(ec);
    }

    return new Error("Identity.Sign", $"Unknown signature algorithm {algorithm.SignatureAlgorithm}");
  }

  private static Result<SigningAlgorithm> ParseRsa(RSA rsa) {
    var modulusSize = rsa.ExportParameters(false).Modulus?.Length;

    if (!IsValidHashSize(modulusSize)) {
      return new Error("Identity.Sign", $"Invalid hash size {modulusSize}. Expected 256, 384 or 512");
    }

    return new SigningAlgorithm($"RS{modulusSize}");
  }

  private static Result<SigningAlgorithm> ParseEc(ECDsa ec) {
    var coordinate = ec.ExportParameters(false).Q.X;
    var hashSize = CalculateEcHashSize(coordinate?.Length ?? 0);

    if (!IsValidHashSize(hashSize)) {
      return new Error("Identity.Sign", $"Invalid hash size {hashSize}. Expected 256, 384 or 512");
    }

    return new SigningAlgorithm($"ES{hashSize}");
  }

  private static int CalculateEcHashSize(int xCoordinateSize) {
    return xCoordinateSize * 8; // 32 - 256, 48 - 384, 64 - 512
  }

  private static bool IsValidHashSize(int? size) {
    if (size is 256 or 384 or 512) {
      return true;
    }

    return false;
  }

  public override string ToString() {
    return _name;
  }

  public static implicit operator string(SigningAlgorithm alg) {
    return alg.ToString();
  }

  public HashAlgorithm GetHasher() {
    return _name switch {
      RS256Value or ES256Value => SHA256.Create(),
      RS384Value or ES384Value => SHA384.Create(),
      RS512Value or ES512Value => SHA512.Create(),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  public static bool operator ==(SigningAlgorithm left, SigningAlgorithm right) {
    return left.Equals(right);
  }

  public static bool operator !=(SigningAlgorithm left, SigningAlgorithm right) {
    return !(left == right);
  }

  public bool Equals(SigningAlgorithm other) {
    return _name == other._name;
  }

  public override bool Equals(object? obj) {
    return obj is SigningAlgorithm other && Equals(other);
  }

  public override int GetHashCode() {
    return _name.GetHashCode();
  }
}

public sealed class SigningAlgorithmJsonConverter : JsonConverter<SigningAlgorithm> {
  public override void WriteJson(JsonWriter writer, SigningAlgorithm value, JsonSerializer serializer) {
    writer.WriteValue(value.ToString());
  }

  public override SigningAlgorithm ReadJson(
    JsonReader reader,
    Type objectType,
    SigningAlgorithm existingValue,
    bool hasExistingValue,
    JsonSerializer serializer
  ) {
    return SigningAlgorithm.Parse(reader.ReadAsString() ?? string.Empty).Value;
  }
}