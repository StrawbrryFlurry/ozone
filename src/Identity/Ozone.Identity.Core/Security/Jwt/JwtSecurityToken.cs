using System.Text;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Security.Jwt;

public sealed class JwtSecurityToken : ISignableSecurityToken {
  internal const int JwtTokenSegmentCount = 3;
  public const string TokenVersion = "1.0";

  public static Result<JwtSecurityToken> Parse(string encodedJwt) {
    if (Error.ArgumentNullOrEmpty(encodedJwt, out var emptyJwtError)) {
      return emptyJwtError;
    }

    var segments = encodedJwt.Split('.');

    return segments
      .ToResult()
      .Ensure(s => s.Length == JwtTokenSegmentCount)
      .OnFailureReturnError(
        new Error("JWT", $"Encoded jwt has {segments.Length} segments but expected {JwtTokenSegmentCount}")
      )
      .MapToMany(
        s => JwtHeader.FromBase64Header(s[0]),
        s => JwtPayload.FromBase64Payload(s[1]),
        s => AssertJwtSignatureNotEmpty(s[2])
      )
      .MergeMap((header, payload, signature) => new JwtSecurityToken(header, payload, signature));
  }

  private static Result<string> AssertJwtSignatureNotEmpty(
    string signature
  ) {
    if (string.IsNullOrEmpty(signature)) {
      return new Error("JWT", "JWT signature is empty");
    }

    return signature;
  }

  internal JwtSecurityToken(JwtHeader header, JwtPayload payload, string signature) {
    Header = header;
    Payload = payload;
    Signature = signature;
  }

  public JwtSecurityToken(
    string issuer,
    string audience,
    string clientId,
    string subject,
    DateTimeOffset issuedAt,
    DateTimeOffset notBefore,
    DateTimeOffset expirationTime
  ) {
    Payload = new JwtPayload(issuer, audience, clientId, subject, issuedAt, notBefore, expirationTime);
  }

  public JwtSecurityToken(
    string issuer,
    string audience,
    string clientId,
    string subject,
    DateTimeOffset expirationTime
  ) {
    Payload = new JwtPayload(issuer, audience, clientId, subject, expirationTime);
  }

  public JwtSecurityToken() {
    Payload = new JwtPayload();
  }

  public JwtPayload Payload { get; }

  /// <summary>
  ///   The header containing signing metadata for this token.
  ///   `null`, ff the token was not yet signed.
  /// </summary>
  public JwtHeader? Header { get; private set; }

  public SigningAlgorithm? SigningAlgorithm => GetSigningAlgorithm();
  public string? SigningKey => Header?.SigningKey;

  /// <summary>
  ///   Gets the "value" of the 'subject' claim { sub, 'value' }.
  /// </summary>
  /// <remarks>If the 'subject' claim is not found, null is returned.</remarks>
  public string Subject => Payload.Subject;

  public DateTimeOffset ValidFrom => Payload.NotBefore;
  public DateTimeOffset ValidTo => Payload.ExpirationTime;

  public string? Signature { get; internal set; }
  public string ClientId => Payload.ClientId;
  public string Issuer => Payload.Issuer;
  public DateTimeOffset IssuedAt => Payload.IssuedAt;

  private SigningAlgorithm? GetSigningAlgorithm() {
    var algorithm = Signing.SigningAlgorithm.Parse(Header?.Algorithm ?? "");

    if (algorithm.IsFailure) {
      return null;
    }

    return algorithm.Value;
  }

  public async Task<Result> SignAsync(ISecurityTokenSigner signer, CancellationToken ct = default) {
    var result = await signer.SignAsBase64Async(this, ct);

    if (result.IsFailure) {
      return result.Error;
    }

    Signature = result.Value.Signature;
    return Result.Success();
  }

  public Result<byte[]> ToSignableHash(SecurityTokenSigningMetadata metadata) {
    var algorithm = metadata.Algorithm;
    Header = new JwtHeader(metadata.KeyId, algorithm);

    var dataToSign = $"{Header.ToBase64Encoded()}.{Payload.ToBase64Encoded()}";
    var hasher = algorithm.GetHasher();

    return hasher.ComputeHash(dataToSign.GetUtf8Bytes());
  }

  public Result<byte[]> ToSignableHash() {
    var algorithm = GetSigningAlgorithm();

    if (algorithm is null) {
      return new Error("Identity.Sign", "Invalid jwt token signature algorithm");
    }

    var signMetadata = new SecurityTokenSigningMetadata() {
      Algorithm = algorithm.Value,
      KeyId = SigningKey
    };

    return ToSignableHash(signMetadata);
  }

  public Result<string> ToSerializedToken() {
    if (Signature is null) {
      return new Error("Identity.Sign", "Cannot serialize token before it was signed. Call <Jwt>.SingAsync first.");
    }

    var jwtBuilder = new StringBuilder();
    jwtBuilder.Append(Header!.ToBase64Encoded());
    jwtBuilder.Append(".");
    jwtBuilder.Append(Payload.ToBase64Encoded());
    jwtBuilder.Append(".");
    jwtBuilder.Append(Signature);

    return jwtBuilder.ToString();
  }
}