using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Authorization;

namespace Ozone.Identity.Core.Security.Jwt;

public ref struct JwtPayloadClaims {
  public const string Audience = "aud";
  public const string Issuer = "iss";
  public const string IssuedAt = "iat";
  public const string NotBefore = "nbf";
  public const string ExpirationTime = "exp";
  public const string ObjectId = "oid";
  public const string TokenVersion = "ver";
  public const string Subject = "sub";
  public const string Scopes = "scp";

  public const string RefreshTokenId = "rti";
  public const string Keychain = "keychain";
  public const string ClientId = "client_id";
  public const string Nonce = "nonce";
}

public sealed class JwtPayload : JwtTokenSegment {
  public JwtPayload()
    : this("", "", "", "", DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now) { }

  public JwtPayload(
    string issuer,
    string audience,
    string clientId,
    string subject,
    DateTimeOffset expirationTime
  ) : this(issuer, audience, clientId, subject, DateTimeOffset.Now, expirationTime, DateTimeOffset.Now) { }

  public JwtPayload(
    string issuer,
    string audience,
    string clientId,
    string subject,
    DateTimeOffset issuedAt,
    DateTimeOffset notBefore,
    DateTimeOffset expirationTime
  ) {
    Add(JwtPayloadClaims.Issuer, issuer);
    Add(JwtPayloadClaims.Audience, audience);
    Add(JwtPayloadClaims.ClientId, clientId);
    Add(JwtPayloadClaims.Subject, subject);
    Add(JwtPayloadClaims.IssuedAt, issuedAt);
    Add(JwtPayloadClaims.NotBefore, notBefore);
    Add(JwtPayloadClaims.ExpirationTime, expirationTime);

    AddInternalClaims();
  }

  public string Audience {
    get => GetRequiredEntry<string>(JwtPayloadClaims.Audience)!;
    set => this[JwtPayloadClaims.Audience] = value;
  }

  public string Issuer {
    get => GetRequiredEntry<string>(JwtPayloadClaims.Issuer)!;
    set => this[JwtPayloadClaims.Issuer] = value;
  }

  public DateTimeOffset IssuedAt {
    get => GetRequiredEntry<DateTimeOffset>(JwtPayloadClaims.IssuedAt);
    set => this[JwtPayloadClaims.IssuedAt] = value;
  }

  public DateTimeOffset NotBefore {
    get => GetRequiredEntry<DateTimeOffset>(JwtPayloadClaims.NotBefore);
    set => this[JwtPayloadClaims.NotBefore] = value;
  }

  public DateTimeOffset ExpirationTime {
    get => GetRequiredEntry<DateTimeOffset>(JwtPayloadClaims.ExpirationTime);
    set => this[JwtPayloadClaims.ExpirationTime] = value;
  }

  public string ClientId {
    get => GetRequiredEntry<string>(JwtPayloadClaims.ClientId)!;
    set => this[JwtPayloadClaims.ClientId] = value;
  }

  public string ObjectId {
    get => GetRequiredEntry<string>(JwtPayloadClaims.ObjectId)!;
    set => this[JwtPayloadClaims.ObjectId] = value;
  }

  public string? Nonce {
    get => GetOptionalEntry<string>(JwtPayloadClaims.Nonce);
    set => this[JwtPayloadClaims.ObjectId] = value;
  }

  /// <summary>
  ///   Returns the ID of the refresh token,
  ///   this token was created with. If the issued token was
  ///   created without a corresponding refresh token, this field is empty
  /// </summary>
  public string? RefreshTokenId {
    get => GetOptionalEntry<string>(JwtPayloadClaims.RefreshTokenId);
    set => this[JwtPayloadClaims.RefreshTokenId] = value;
  }

  public AuthorizationScopes Scopes =>
    GetRequiredEntryFormat(
      JwtPayloadClaims.Scopes,
      (scopes) => AuthorizationScopes.CreateFrom((string)scopes).Value
    );

  /// <summary>
  ///   A unique identifier for an identity of an application.
  ///   Opposed to the ObjectId, which uniquely identifies a principal
  ///   across the whole service ecosystem, this identifier couples the
  ///   user identity with a given service.
  /// </summary>
  public string Subject {
    get => GetRequiredEntry<string>(JwtPayloadClaims.Subject)!;
    set => this[JwtPayloadClaims.Subject] = value;
  }

  public string? TokenVersion => GetOptionalEntry<string>(JwtPayloadClaims.TokenVersion);

  public AuthorizationKeyChain KeyChain => GetRequiredEntryFormat(
    JwtPayloadClaims.Keychain,
    (keychain) => AuthorizationKeyChain.CreateFrom((string)keychain).Value
  );

  public static Result<JwtPayload> FromBase64Payload(string payload) {
    try {
      return JsonConvert.DeserializeObject<JwtPayload>(payload.FromBase64UrlEncoded())!;
    }
    catch (Exception ex) {
      return new Error("", "");
    }
  }

  private void AddInternalClaims() {
    Add(JwtPayloadClaims.TokenVersion, JwtSecurityToken.TokenVersion);
    Add(JwtPayloadClaims.Scopes, "");
    Add(JwtPayloadClaims.Keychain, "");
  }
}