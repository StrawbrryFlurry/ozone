using Microsoft.IdentityModel.Tokens;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Core.Security.Jwt;
using Ozone.Identity.Core.Security.Signing;
using Ozone.Testing.Common.FluentExtensions;
using Ozone.Testing.Common.Time;

namespace Ozone.Identity.Core.Tests.Unit.Security.Jwt;

public sealed class JwtSecurityTokenTests {
  private const string JwtHeaderString = "eyJ0eXAiOiJKV1QiLCJraWQiOiJLZXlJZCIsImFsZyI6IkVTMjU2In0";

  private const string JwtPayloadString =
    "eyJpc3MiOiJvem9uZS1pZGVudGl0eSIsImF1ZCI6IkFwcGxpY2F0aW9uSWQiLCJjbGllbnRfaWQiOiJBcHBsaWNhdGlvbklkIiwic3ViIjoiVXNlcklkOjpBcHBsaWNhdGlvbklkIiwiaWF0Ijo5NzE2NTQ0MDAsIm5iZiI6OTcxNjU0NDAwLCJleHAiOjk3MTY1NDQwMCwidmVyIjoidjEuMCIsInNjcCI6IlNjb3BlMSBTY29wZTIiLCJrZXljaGFpbiI6IkFjdGlvbjEgQWN0aW9uMiIsIm5hbWUiOiJKb2huIERvZSIsIm9pZCI6IlVzZXJJZCIsInJ0aSI6IlJlZnJlc2hUb2tlbklkIn0";

  private const string JwtSignatureString =
    "lW3hvPX7Nlkf2gCcF0Eh8OPEblFnr6mzN_g_UEesJxjaAETbEInmkuIz0bTaubLvSTWPapvD6VgZ9Z79yHzr2w";

  private const string JwtString = $"{JwtHeaderString}.{JwtPayloadString}.{JwtSignatureString}";

  private const string IdTokenString =
    "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjFMVE16YWtpaGlSbGFfOHoyQkVKVlhlV01xbyJ9.eyJ2ZXIiOiIyLjAiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vOTEyMjA0MGQtNmM2Ny00YzViLWIxMTItMzZhMzA0YjY2ZGFkL3YyLjAiLCJzdWIiOiJBQUFBQUFBQUFBQUFBQUFBQUFBQUFJa3pxRlZyU2FTYUZIeTc4MmJidGFRIiwiYXVkIjoiNmNiMDQwMTgtYTNmNS00NmE3LWI5OTUtOTQwYzc4ZjVhZWYzIiwiZXhwIjoxNTM2MzYxNDExLCJpYXQiOjE1MzYyNzQ3MTEsIm5iZiI6MTUzNjI3NDcxMSwibmFtZSI6IkFiZSBMaW5jb2xuIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiQWJlTGlAbWljcm9zb2Z0LmNvbSIsIm9pZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC02NmYzLTMzMzJlY2E3ZWE4MSIsInRpZCI6IjkxMjIwNDBkLTZjNjctNGM1Yi1iMTEyLTM2YTMwNGI2NmRhZCIsIm5vbmNlIjoiMTIzNTIzIiwiYWlvIjoiRGYyVVZYTDFpeCFsTUNXTVNPSkJjRmF0emNHZnZGR2hqS3Y4cTVnMHg3MzJkUjVNQjVCaXN2R1FPN1lXQnlqZDhpUURMcSFlR2JJRGFreXA1bW5PcmNkcUhlWVNubHRlcFFtUnA2QUlaOGpZIn0.1AFWW-Ck5nROwSlltm7GzZvDwUkqvhSQpm55TQsmVo9Y59cLhRXpvB8n-55HCr9Z6G_31_UbeUkoz612I2j_Sm9FFShSDDjoaLQr54CreGIJvjtmS3EkK9a7SJBbcpL1MpUtlfygow39tFjY7EVNW9plWUvRrTgVk7lYLprvfzw-CIqw3gHC-T7IK_m_xkr08INERBtaecwhTeN4chPC4W3jdmw_lIxzC48YoQ0dB1L9-ImX98Egypfrlbm0IBL5spFzL6JDZIRRJOu8vecJvj1mq-IUhGt0MacxX8jdxYLP-KUu2d9MbNKpCKJuZ7p8gwTL5B7NlUdh_dmSviPWrw";

  private static readonly JwtHeader ParsedHeader = JwtHeader.FromBase64Header(JwtHeaderString).Value;
  private static readonly JwtPayload ParsedPayload = JwtPayload.FromBase64Payload(JwtPayloadString).Value;

  private static readonly dynamic JwtPayloadData = new {
    Type = "JWT",
    Algorithm = SigningAlgorithm.Ec256,
    KeyId = "KeyId",
    Subject = "UserId::ApplicationId",
    Name = "John Doe",
    IssuedAt = TimeUtils.TestTime,
    Audience = "ApplicationId",
    Issuer = "ozone-identity",
    Scopes = AuthorizationScopes.CreateFrom("Scope1 Scope2").Value,
    ClientId = "ApplicationId",
    ExpirationTime = TimeUtils.TestTime,
    KeyChain = AuthorizationKeyChain.CreateFrom("Action1 Action2").Value,
    NotBefore = TimeUtils.TestTime,
    ObjectId = "UserId",
    TokenVersion = "v1.0",
    RefreshTokenId = "RefreshTokenId"
  };

  [Fact]
  public void Ctor_CreatesPayloadAndAddsItToToken() {
    var sut = new JwtSecurityToken();

    sut.Payload.Should().NotBeNull();
  }

  [Fact]
  public void Parse_ReturnsError_WhenArgumentIsEmptyString() {
    var sut = JwtSecurityToken.Parse("");

    sut.ShouldBeFailure();
  }

  [Fact]
  public void Parse_ReturnsError_WhenJwtDesNotContainThreeParts() {
    var sut = JwtSecurityToken.Parse("A.B");

    sut.ShouldBeFailure();
  }

  [Fact]
  public void Parse_ReturnsError_WhenJwtHeaderIsInvalid() {
    var sut = JwtSecurityToken.Parse($"Foo.{JwtPayloadString}.Signature");

    sut.ShouldBeFailure();
  }

  [Fact]
  public void Parse_ReturnsError_PayloadIsInvalid() {
    var sut = JwtSecurityToken.Parse($"{JwtHeaderString}.Foo.Signature");

    sut.ShouldBeFailure();
  }

  [Fact]
  public void Parse_ParsesJwtAndAssignsHeaderAndPayload_WhenArgumentIsSingleJwtString() {
    var sut = JwtSecurityToken.Parse(JwtString).Value;

    sut.Header.Type.Should().Be(JwtPayloadData.Type);
    sut.Header.Algorithm.Should().Be(JwtPayloadData.Algorithm);
    sut.Header.SigningKey.Should().Be(JwtPayloadData.KeyId);

    sut.Payload.Subject.Should().Be(JwtPayloadData.Subject);
    sut.Payload.Audience.Should().Be(JwtPayloadData.Audience);
    sut.Payload.Issuer.Should().Be(JwtPayloadData.Issuer);
    sut.Payload.Scopes.Should().BeEquivalentTo(JwtPayloadData.Scopes);
    sut.Payload.ClientId.Should().Be(JwtPayloadData.ClientId);
    sut.Payload.ExpirationTime.Should().Be(JwtPayloadData.ExpirationTime);
    sut.Payload.IssuedAt.Should().Be(JwtPayloadData.IssuedAt);
    sut.Payload.KeyChain.Should().BeEquivalentTo(JwtPayloadData.KeyChain);
    sut.Payload.NotBefore.Should().Be(JwtPayloadData.NotBefore);
    sut.Payload.ObjectId.Should().Be(JwtPayloadData.ObjectId);
    sut.Payload.TokenVersion.Should().Be(JwtPayloadData.TokenVersion);
    sut.Payload.RefreshTokenId.Should().Be(JwtPayloadData.RefreshTokenId);

    sut.Signature.Should().Be(JwtSignatureString);
  }

  [Fact]
  public void Parse_AssignesHeaderAndPayload_WhenTokenIsIdToken() {
    var sut = JwtSecurityToken.Parse(IdTokenString).Value;

    sut.Header.Type.Should().Be("JWT");
    sut.Header.Algorithm.Should().Be(SigningAlgorithm.RS256);
    sut.Payload["preferred_username"].Should().Be("AbeLi@microsoft.com");
    sut.Payload.Nonce.Should().Be("123523");
  }

  [Fact]
  public void SigningAlgorithm_ReturnsNull_WhenAlgorithmIsUnknown() {
    var header = new JwtHeader() {
      { "alg", "Foo" }
    };

    var sut = new JwtSecurityToken(
      header,
      JwtPayload.FromBase64Payload(JwtPayloadString).Value,
      JwtSignatureString
    );

    sut.SigningAlgorithm.Should().BeNull();
  }

  [Fact]
  public void SigningAlgorithm_ReturnsParsedAlgorithm_WhenAlgorithmIsValid() {
    var header = new JwtHeader() {
      { "alg", SigningAlgorithm.RS256.ToString() }
    };

    var sut = new JwtSecurityToken(
      header,
      JwtPayload.FromBase64Payload(JwtPayloadString).Value,
      JwtSignatureString
    );

    sut.SigningAlgorithm.Should().Be(SigningAlgorithm.RS256);
  }

  [Fact]
  public void ToSignableHash_ReturnsError_WhenHeaderDoesNotExistAndNoAlgorithmIsSpecified() {
    var header = new JwtHeader() {
      { "alg", null }
    };

    var sut = new JwtSecurityToken(
      header,
      JwtPayload.FromBase64Payload(JwtPayloadString).Value,
      JwtSignatureString
    );

    sut.ToSignableHash().ShouldBeFailure();
  }

  [Fact]
  public void ToSignableHash_ReturnsHashOfHeaderAndPayload_WhenAlgorithmIsSpecifiedThroughMetadata() {
    var header = new JwtHeader() {
      { "alg", SigningAlgorithm.RS256.ToString() }
    };

    var sut = new JwtSecurityToken(
      header,
      JwtPayload.FromBase64Payload(JwtPayloadString).Value,
      JwtSignatureString
    );

    var signingMetadata = new SecurityTokenSigningMetadata() {
      Algorithm = SigningAlgorithm.Ec256,
      KeyId = "KeyId"
    };

    sut.ToSignableHash(signingMetadata).ShouldBeSuccessful();
  }

  [Fact]
  public void ToSignableHash_ReturnsHashOfHeaderAndPayload_WhenAlgorithmIsDefinedInHeader() {
    var header = new JwtHeader() {
      { JwtHeaderField.Alg, SigningAlgorithm.RS256.ToString() },
      { JwtHeaderField.Kid, "KeyId" }
    };

    var sut = new JwtSecurityToken(
      header,
      ParsedPayload,
      JwtSignatureString
    );

    var hashingAlgorithm = sut.SigningAlgorithm!.Value.GetHasher();
    var hashData = $"{header.ToBase64Encoded()}.{ParsedPayload.ToBase64Encoded()}";
    var expectedHash = hashingAlgorithm.ComputeHash(hashData.GetUtf8Bytes());

    var actual = sut.ToSignableHash().Value;
    actual.Should().BeEquivalentTo(expectedHash);
  }

  [Fact]
  public void ToSerializedToken_ReturnsBase64EncodedToken() {
    var sut = new JwtSecurityToken(
      JwtHeader.FromBase64Header(JwtHeaderString).Value,
      JwtPayload.FromBase64Payload(JwtPayloadString).Value,
      JwtSignatureString
    );

    sut.ToSerializedToken().Value.Should().Be(JwtString);
  }
}