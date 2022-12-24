using System.Net.Mime;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ozone.Common.Core.Abstractions;
using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Functional;
using Ozone.Common.Identification;
using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Authorization.Provider.OAuth;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Core.Security.OAuth;
using Ozone.Identity.Core.Tests.Unit.Stubs;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Identity.ValueObjects;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;
using Ozone.Testing.Common.FluentExtensions;
using Ozone.Testing.Common.Mocking;
using Ozone.Testing.Common.Time;

namespace Ozone.Identity.Core.Tests.Unit.Authorization.Provider.OAuth;

public sealed class OAuthAuthenticationProviderTests {
  private static readonly OAuthAuthenticationProviderOptions Options = new() {
    Authority = "authority",
    ClientId = "client-id",
    ClientSecret = "client-secret",
    Instance = "oauth.io",
    AuthorizeUri = "https://oauth.io/authorize",
    OzoneAuthroizeUri = "https://login.ozone.io/authorize"
  };

  private const string ClientId = "eb5e29dc-9d38-4140-91a4-8a9c7bd16864";
  private static readonly Guid ClientIdGuid = Guid.Parse(ClientId);
  private const string State = "1234";
  private const string IdpIdTokenNonce = "123523";
  private const string UserId = "00000000-0000-0000-66f3-3332eca7ea81";
  private static readonly CorrelationId CorrelationId = new();
  private static readonly Uri RedirectUri = new("https://client-app.redirect");

  private static AuthorizationRequest Request => new() {
    CorrelationId = CorrelationId,
    ClientId = ClientIdGuid,
    State = State,
    CodeChallenge = "aabbcc",
    CodeChallengeMethod = CodeChallengeMode.SHA256,
    RedirectUri = RedirectUri,
    Scope = AuthorizationScopes.CreateFrom("openid Test.Scope").Value,
    ResponseMode = ResponseMode.Query,
    IdentityProvider = "msal",
    RequestedKeyChain = AuthorizationKeyChain.CreateFrom("service.action.2").Value
  };

  public const string IdTokenJwt =
    "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjFMVE16YWtpaGlSbGFfOHoyQkVKVlhlV01xbyJ9.eyJ2ZXIiOiIyLjAiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vOTEyMjA0MGQtNmM2Ny00YzViLWIxMTItMzZhMzA0YjY2ZGFkL3YyLjAiLCJzdWIiOiJBQUFBQUFBQUFBQUFBQUFBQUFBQUFJa3pxRlZyU2FTYUZIeTc4MmJidGFRIiwiYXVkIjoiNmNiMDQwMTgtYTNmNS00NmE3LWI5OTUtOTQwYzc4ZjVhZWYzIiwiZXhwIjoxNTM2MzYxNDExLCJpYXQiOjE1MzYyNzQ3MTEsIm5iZiI6MTUzNjI3NDcxMSwibmFtZSI6IkFiZSBMaW5jb2xuIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiQWJlTGlAbWljcm9zb2Z0LmNvbSIsIm9pZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC02NmYzLTMzMzJlY2E3ZWE4MSIsInRpZCI6IjkxMjIwNDBkLTZjNjctNGM1Yi1iMTEyLTM2YTMwNGI2NmRhZCIsIm5vbmNlIjoiMTIzNTIzIiwiYWlvIjoiRGYyVVZYTDFpeCFsTUNXTVNPSkJjRmF0emNHZnZGR2hqS3Y4cTVnMHg3MzJkUjVNQjVCaXN2R1FPN1lXQnlqZDhpUURMcSFlR2JJRGFreXA1bW5PcmNkcUhlWVNubHRlcFFtUnA2QUlaOGpZIn0.1AFWW-Ck5nROwSlltm7GzZvDwUkqvhSQpm55TQsmVo9Y59cLhRXpvB8n-55HCr9Z6G_31_UbeUkoz612I2j_Sm9FFShSDDjoaLQr54CreGIJvjtmS3EkK9a7SJBbcpL1MpUtlfygow39tFjY7EVNW9plWUvRrTgVk7lYLprvfzw-CIqw3gHC-T7IK_m_xkr08INERBtaecwhTeN4chPC4W3jdmw_lIxzC48YoQ0dB1L9-ImX98Egypfrlbm0IBL5spFzL6JDZIRRJOu8vecJvj1mq-IUhGt0MacxX8jdxYLP-KUu2d9MbNKpCKJuZ7p8gwTL5B7NlUdh_dmSviPWrw";

  private static ExternalAuthenticationCallback Callback => new() {
    CorrelationId = CorrelationId,
    IdToken = IdTokenJwt,
    Code = "aabbcc"
  };

  private readonly Mock<IOAuthTokenClient> _tokenClientMock = new();
  private Mock<IOzoneUserRepository> _userRepositoryMock = new();
  private Mock<IExternalAuthenticationChallengeRepository> _authenticationChallengeRepositoryMock = new();
  private Mock<IAuthorizationGrantRepository> _authorizationGrantRepositoryMock = new();
  private Mock<IServiceApplicationRepository> _serviceApplicationRepositoryMock = new();
  private Mock<IUnitOfWork> _unitOfWorkMock = new();

  public static ServiceAction ServiceAction1 =
    ServiceActionStub.Create("service.action.1", "Test Action 1", ClientIdGuid);

  public static ServiceAction ServiceAction2 =
    ServiceActionStub.Create("service.action.2", "Test Action 2", ClientIdGuid);

  public Scope OpenIdScope = ScopeStub.Create("openid", "Read your basic profile", true);

  public Scope TestScope = ScopeStub.Create("Test.Scope", "Test scope", ClientIdGuid, new List<ServiceAction> {
    ServiceAction1
  });

  private ExternalAuthenticationChallenge Challenge => EntityMockFactory
    .CreateInstance<ExternalAuthenticationChallenge>()
    .SetPropertyBackingField(x => x.CorrelationId, CorrelationId)
    .SetPropertyBackingField(x => x.State, State)
    .SetPropertyBackingField(x => x.RedirectUri, RedirectUri)
    .SetPropertyBackingField(x => x.IdpNonce, IdpIdTokenNonce)
    .SetPropertyBackingField(x => x.ExpiresAtUtc, DateTimeOffset.MaxValue)
    .SetPropertyBackingField(x => x.ClientApplication,
      ServiceApplicationStub.Create(ClientIdGuid, "Test Application", "Some Test Application"))
    .SetPropertyBackingField(x => x.Scopes, new List<Scope> {
      OpenIdScope,
      TestScope
    })
    .SetPropertyBackingField(x => x.Keychain, new List<ServiceAction> {
      ServiceAction2
    });

  public OAuthAuthenticationProviderTests() {
    _serviceApplicationRepositoryMock
      .Setup(x => x.GetServiceApplicationByIdAsync(It.IsAny<Guid>()))
      .ReturnsAsync(Challenge.ClientApplication);

    _authenticationChallengeRepositoryMock
      .Setup(x => x.AddAsync(It.IsAny<ExternalAuthenticationChallenge>()))
      .Returns(Task.CompletedTask);

    _serviceApplicationRepositoryMock
      .Setup(x => x.GetScopesByDescriptorAsync(It.IsAny<ServiceApplication>(), It.IsAny<AuthorizationScopes>()))
      .ReturnsAsync(
        new List<Scope> {
          OpenIdScope,
          TestScope
        }
      );

    _serviceApplicationRepositoryMock
      .Setup(x => x.GetServiceActionsByDescriptorAsync(It.IsAny<ServiceApplication>(),
        It.IsAny<AuthorizationKeyChain>()))
      .ReturnsAsync(
        new List<ServiceAction> {
          ServiceAction1,
          ServiceAction2
        }
      );

    _authenticationChallengeRepositoryMock
      .Setup(x => x.GetByCorrelationIdAsync(CorrelationId))
      .ReturnsAsync(Challenge);

    _userRepositoryMock
      .Setup(x => x.GetUserIdentityAsync(It.IsAny<UserIdentifier>()))
      .ReturnsAsync(OzoneUserStub.Create(
        UserId,
        "Test User",
        new List<OzoneRole>() {
          OzoneRoleStub.Create("Test.Role1", ClientIdGuid, new List<ServiceAction>() {
            ServiceAction1,
            ServiceAction2
          })
        }
      ));

    _tokenClientMock
      .Setup(x => x.GetTokenByCodeAsync(It.IsAny<OAuthCodeTokenRequest>()))
      .ReturnsAsync(new OAuthTokenResponse() {
        TokenType = "Bearer",
        AccessToken = IdTokenJwt,
        RefreshToken = "refresh-token"
      });
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_ReturnsError_WhenRequestHasInvalidParameters() {
    var sut = CreateSut();
    var request = Request.SetPropertyBackingField(
      r => r.ExternalProviderParameters,
      new ParameterCollection() {
        { "fail", "testing parameter to make the parameter check fail" }
      });

    var result = await sut.CreateAuthorizationTicketAsync(request, CancellationToken.None);

    result.ShouldFailWith(ApplicationErrors.Authentication.InvalidParameter("fail"));
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_ReturnsError_WhenRequestedScopeDoesNotExist() {
    var sut = CreateSut();

    var error = ApplicationErrors.Authorization.InvalidAuthorizationScope("Test.NonExistingScope");
    _serviceApplicationRepositoryMock
      .Setup(x => x.GetScopesByDescriptorAsync(It.IsAny<ServiceApplication>(), It.IsAny<AuthorizationScopes>()))
      .ReturnsAsync(error);

    var result = await sut.CreateAuthorizationTicketAsync(Request, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_ReturnsError_WhenRequestedKeyChainScopeDoesNotExist() {
    var sut = CreateSut();

    var error = ApplicationErrors.Authorization.InvalidKeyChainAction("action.unknown");
    _serviceApplicationRepositoryMock
      .Setup(x => x.GetServiceActionsByDescriptorAsync(It.IsAny<ServiceApplication>(),
        It.IsAny<AuthorizationKeyChain>()))
      .ReturnsAsync(error);

    var result = await sut.CreateAuthorizationTicketAsync(Request, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_CreatesPKCEChallengeForRemoteAuthentication() {
    var sut = CreateSut();

    var result = await sut.CreateAuthorizationTicketAsync(Request, CancellationToken.None);
    result.ShouldBeSuccessful();

    var challenge = _authenticationChallengeRepositoryMock
      .FirstInvocationOfName(x => x.AddAsync)
      .GetArgument<ExternalAuthenticationChallenge>();

    challenge.IdpCodeChallenge.Should().NotBeNullOrWhiteSpace();
    challenge.IdpCodeChallengeVerifier.Should().NotBeNullOrWhiteSpace();

    var challengeMethod = challenge.IdpCodeChallengeMode;
    var verifier = challenge.IdpCodeChallengeVerifier;
    var codeChallenge = challenge.IdpCodeChallenge;

    challengeMethod.Should().Be(sut.CodeChallengeMode);
    var hasher = challengeMethod.GetHashAlgorithm();
    var hash = hasher.ComputeHash(verifier.GetUtf8Bytes());
    hash.ToBase64UrlEncoded().Should().Be(codeChallenge);
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_SavesExternalAuthenticationChallenge() {
    var sut = CreateSut();

    var request = Request;
    var result = await sut.CreateAuthorizationTicketAsync(request, CancellationToken.None);
    result.ShouldBeSuccessful();

    var challenge = _authenticationChallengeRepositoryMock
      .FirstInvocationOfName(x => x.AddAsync)
      .GetArgument<ExternalAuthenticationChallenge>();

    challenge.IdpCodeChallengeVerifier.Should().NotBeNullOrWhiteSpace();
    challenge.IdpNonce.Should().NotBeNullOrWhiteSpace();
    challenge.IdpCodeChallenge.Should().NotBeNullOrWhiteSpace();
    challenge.IdpState.Should().Be(request.CorrelationId);
    challenge.CodeChallenge.Should().Be(request.CodeChallenge);
    challenge.CodeChallengeMode.Should().Be(request.CodeChallengeMethod);
    challenge.CorrelationId.Should().Be(request.CorrelationId);
    challenge.IdentityProvider.Should().Be(request.IdentityProvider);
    challenge.ResponseMode.Should().Be(request.ResponseMode);
    challenge.RedirectUri.Should().Be(request.RedirectUri);
    challenge.Keychain
      .Select(x => x.Name)
      .Should().Contain(
        request.RequestedKeyChain.Select(a => a.Descriptor)
      );
    challenge.Scopes.Select(x => x.ScopeDescriptor).Should().Contain(request.Scope.ToList());
  }

  [Fact]
  public async Task CreateAuthorizationTicketAsync_ReturnsOAuthAuthorizationTicket_WhenRequestIsValid() {
    var sut = CreateSut();

    var result = await sut.CreateAuthorizationTicketAsync(Request, CancellationToken.None);

    result.ShouldBeSuccessful();

    var challenge = _authenticationChallengeRepositoryMock
      .FirstInvocationOfName(x => x.AddAsync)
      .GetArgument<ExternalAuthenticationChallenge>();

    var ticket = (RedirectAuthorizationTicket)result.Value;
    var redirectUri = new Uri(ticket.RedirectUri);
    var query = HttpUtility.ParseQueryString(redirectUri.Query);

    redirectUri.Host.Should().Be(Options.Instance);
    redirectUri.AbsolutePath.Should().Be("/authorize");

    query["client_id"].Should().Be(Options.ClientId);
    query["redirect_uri"].Should().Be($"https://oauth.io/authorize/callback/{sut.IdentityProvider}");
    query["scope"].Should().Be("openid");
    query["response_type"].Should().Be("code id_token");
    query["state"].Should().Be(challenge.IdpState);
    query["nonce"].Should().Be(challenge.IdpNonce);
    query["code_challenge"].Should().Be(challenge.IdpCodeChallenge);
    query["code_challenge_method"].Should().Be(challenge.IdpCodeChallengeMode.ToString());
    query["response_mode"].Should().Be(ResponseMode.Query.ToString());
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsError_WhenCorrelationIdIsNotFound() {
    var sut = CreateSut();

    var error = DomainErrors.Auth.ExternalAuthenticationChallengeNotFound(CorrelationId);
    _authenticationChallengeRepositoryMock
      .Setup(x => x.GetByCorrelationIdAsync(CorrelationId))
      .ReturnsAsync(error);

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsError_WhenChallengeWithCorrelationIdIsExpired() {
    var sut = CreateSut();

    var error = ApplicationErrors.Authorization.ExternalAuthenticationChallengeExpired(CorrelationId);
    _authenticationChallengeRepositoryMock
      .Setup(x => x.GetByCorrelationIdAsync(CorrelationId))
      .ReturnsAsync(error);

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenExternalAuthenticationFailed() {
    var sut = CreateSut();

    var callback = Callback
      .SetPropertyBackingField(x => x.ErrorDescription, "Oops, something went wrong");

    var result = await sut.HandleAuthenticationCallbackAsync(callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(
      result,
      ApplicationErrors.Authorization.ExternalIdentityProviderAuthenticationFailed(callback.ErrorDescription)
    );
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenIdTokenIsInvalid() {
    var sut = CreateSut();

    var callback = Callback
      .SetPropertyBackingField(x => x.IdToken, "invalid_token");

    var result = await sut.HandleAuthenticationCallbackAsync(callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, ApplicationErrors.Authorization.ExternalIdentityProviderInvalidIdToken);
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenIdTokenHasInvalidNonce() {
    var sut = CreateSut();

    var challenge = Challenge
      .SetPropertyBackingField(x => x.IdpNonce, "SomeOtherNonce");

    _authenticationChallengeRepositoryMock
      .Setup(x => x.GetByCorrelationIdAsync(CorrelationId))
      .ReturnsAsync(challenge);

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, ApplicationErrors.Authorization.ExternalIdentityProviderIdTokenInvalidNonce);
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenUserDoesNotPermissionsToGrantScope() {
    var sut = CreateSut();

    _userRepositoryMock
      .Setup(x => x.GetUserIdentityAsync(It.IsAny<UserIdentifier>()))
      .ReturnsAsync(OzoneUserStub.Create(
        UserId,
        "Test User",
        new List<OzoneRole>() {
          OzoneRoleStub.Create("Test.Role1", ClientIdGuid, new List<ServiceAction>() {
            ServiceAction2
          })
        }
      ));

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, DomainErrors.UserIdentity.UnauthorizedScope(TestScope.ScopeDescriptor));
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenUserDoesNotPermissionsToGrantServiceAction() {
    var sut = CreateSut();

    _userRepositoryMock
      .Setup(x => x.GetUserIdentityAsync(It.IsAny<UserIdentifier>()))
      .ReturnsAsync(OzoneUserStub.Create(
        UserId,
        "Test User",
        new List<OzoneRole>() {
          OzoneRoleStub.Create("Test.Role1", ClientIdGuid, new List<ServiceAction>() {
            ServiceAction1
          })
        }
      ));

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, DomainErrors.UserIdentity.UnauthorizedServiceAction("service.action.2"));
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenExternalOAuthCodeTokenRequestFails() {
    var sut = CreateSut();

    var error = ApplicationErrors.Authorization.ExternalIdentityProviderError("Oops, something went wrong");
    _tokenClientMock
      .Setup(x => x.GetTokenByCodeAsync(It.IsAny<OAuthCodeTokenRequest>()))
      .ReturnsAsync(error);

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, error);
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenExternalOAuthServerDoesNotReturnRefreshTokenForCode() {
    var sut = CreateSut();

    _tokenClientMock
      .Setup(x => x.GetTokenByCodeAsync(It.IsAny<OAuthCodeTokenRequest>()))
      .ReturnsAsync(new OAuthTokenResponse() {
        TokenType = "Bearer",
        AccessToken = "foo"
      });

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(result, ApplicationErrors.Authorization.ExternalIdentityProviderInvalidRefreshToken);
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsErrorRedirect_WhenExternalOAuthServerDoesNotReturnAccessTokenWithSameOID() {
    var sut = CreateSut();

    _tokenClientMock
      .Setup(x => x.GetTokenByCodeAsync(It.IsAny<OAuthCodeTokenRequest>()))
      .ReturnsAsync(new OAuthTokenResponse() {
        TokenType = "Bearer",
        AccessToken =
          "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJvaWQiOiJzb21lLXVzZXItaWQifQ.sign",
        RefreshToken = "bar"
      });

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);

    ShouldSucceedWithErrorRedirect(
      result,
      ApplicationErrors.Authorization.ExternalIdentityProviderAccessTokenObjectIdDidNotMatchIdToken(
        UserId,
        "some-user-id"
      )
    );
  }

  [Fact]
  public async Task HandleAuthenticationCallbackAsync_GeneratesAuthCodeAndSavesIt() {
    var sut = CreateSut();

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);
    result.ShouldBeSuccessful();

    var code = _authorizationGrantRepositoryMock
      .FirstInvocationOfName(x => x.AddAuthorizationCodeAsync)
      .GetArgument<AuthorizationCode>();

    var challenge = Challenge;

    code.Code.Should().NotBeEmpty();
    code.Identity.Id.UserId.Should().Be(UserId);
    code.Scopes.Should().BeEquivalentTo(challenge.Scopes);
    code.KeyChain.Should().BeEquivalentTo(challenge.Keychain);
    code.RedirectUri.Should().Be(challenge.RedirectUri);
    code.CorrelationId.Should().BeEquivalentTo(challenge.CorrelationId);
    code.ClientApplication.Should().Be(challenge.ClientApplication);
    code.IdpRefreshToken.Should().Be("refresh-token");
    code.CodeChallenge.Should().Be(challenge.CodeChallenge);
    code.CodeChallengeMode.Should().Be(challenge.CodeChallengeMode);
    code.IdentityProvider.Should().Be(challenge.IdentityProvider);
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsOzoneAuthorizationRedirectTicket_WhenUserHasNotGrantedAccessToApplication() {
    var sut = CreateSut();

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);
    result.ShouldBeSuccessful();

    var code = _authorizationGrantRepositoryMock
      .FirstInvocationOfName(x => x.AddAuthorizationCodeAsync)
      .GetArgument<AuthorizationCode>();

    var ticket = result.Value as RedirectAuthorizationTicket;
    var redirectUri = new Uri(ticket.RedirectUri);
    var query = HttpUtility.ParseQueryString(redirectUri.Query);

    redirectUri.OriginalString.Should().StartWith(Options.OzoneAuthroizeUri);
    query["code"].Should().Be(code.Code);
    query["state"].Should().Be(State);
    query["client_name"].Should().Be(code.ClientApplication.Name);
    query["client_description"].Should().Be(code.ClientApplication.Description);
    query["username"].Should().Be(code.Identity.Username);
    query["scopes"].Should().Be("""
    [{"name":"service.action.2","description":"Test Action 2"},{"name":"openid","description":"Read your basic profile"},{"name":"Test.Scope","description":"Test scope"}]
    """);
    query["redirect_uri"].Should().Be(RedirectUri.ToString());
  }

  [Fact]
  public async Task
    HandleAuthenticationCallbackAsync_ReturnsClientRedirectAuthenticationTicket_WhenUserHasAlreadyGrantedAccessToTheApplication() {
    var sut = CreateSut();

    _authorizationGrantRepositoryMock
      .Setup(x => x.GetGrantAsync(It.IsAny<OzoneUser>(), It.IsAny<ServiceApplication>()))
      .ReturnsAsync(AuthorizationGrantStub.Create(new List<ServiceAction> {
        ServiceAction1,
        ServiceAction2
      }));

    var result = await sut.HandleAuthenticationCallbackAsync(Callback, CancellationToken.None);
    result.ShouldBeSuccessful();

    var code = _authorizationGrantRepositoryMock
      .FirstInvocationOfName(x => x.AddAuthorizationCodeAsync)
      .GetArgument<AuthorizationCode>();

    var ticket = result.Value as RedirectAuthorizationTicket;
    var redirectUri = new Uri(ticket.RedirectUri);
    var query = HttpUtility.ParseQueryString(redirectUri.Query);

    redirectUri.OriginalString.Should().StartWith(RedirectUri.ToString());
    query["code"].Should().Be(code.Code);
    query["state"].Should().Be(State);
  }

  public static void ShouldSucceedWithErrorRedirect(Result<IAuthorizationTicket> result, Error error) {
    result.ShouldSucceedWithEquivalent(
      RedirectAuthorizationTicket.FromError(
        RedirectUri,
        State,
        error
      )
    );
  }

  private OAuthAuthenticationProvider CreateSut() {
    return new OAuthAuthenticationProviderImpl(
      Options,
      TimeUtils.TestClock(),
      _tokenClientMock.Object,
      _userRepositoryMock.Object,
      _authenticationChallengeRepositoryMock.Object,
      _authorizationGrantRepositoryMock.Object,
      _serviceApplicationRepositoryMock.Object,
      _unitOfWorkMock.Object
    );
  }
}