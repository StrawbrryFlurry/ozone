using Moq;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Domain.Errors;
using Ozone.Testing.Common.Extensions;
using Ozone.Testing.Common.FluentExtensions;

namespace Ozone.Identity.Core.Tests.Unit.Authorization.Commands.AuthorizationReqeust;

public sealed class AuthorizationCommandHandlerTests {
  private readonly Mock<IAuthenticationProviderCollection> _authorizationHandlerCollectionMock = new();
  private readonly Mock<IAuthenticationProvider<IAuthenticationProviderOptions>> _authenticationProviderMock = new();

  private const string ClientId = "eb5e29dc-9d38-4140-91a4-8a9c7bd16864";

  private static AuthorizeCommand AuthorizeCommand => new() {
    ClientId = ClientId,
    State = "1234",
    CodeChallenge = "aabbcc",
    CodeChallengeMethod = "S256",
    RedirectUri = new Uri("https://client-app.redirect"),
    Scope = "openid profile email",
    ResponseMode = "query",
    IdentityProvider = "msal",
    KeyChain = "Service.Action"
  };

  public AuthorizationCommandHandlerTests() {
    _authorizationHandlerCollectionMock
      .Setup(x => x.GetProvider(It.IsAny<string>()))
      .Returns(Result.Success(_authenticationProviderMock.Object));
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenCodeChallengeModeIsInvalid() {
    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );
    var command = AuthorizeCommand.SetPropertyBackingField(c => c.CodeChallengeMethod, "invalid");

    var result = await sut.Handle(command);

    result.ShouldFailWith(DomainErrors.ValueObject.InvalidCodeChallengeMode);
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenResponseModeIsInvalid() {
    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );
    var command = AuthorizeCommand.SetPropertyBackingField(c => c.ResponseMode, "invalid");

    var result = await sut.Handle(command);

    result.ShouldFailWith(DomainErrors.ValueObject.InvalidResponseMode);
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenKeyChainActionContainsInvalidCharacters() {
    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );
    var command = AuthorizeCommand.SetPropertyBackingField(c => c.KeyChain, "inv{alid");

    var result = await sut.Handle(command);

    result.ShouldFailWith(ApplicationErrors.Authorization.InvalidKeyChainActionFormat("inv{alid"));
  }

  [Fact(Skip = "Currently, there is no way for the scope to be invalid")]
  public async Task Handle_ReturnsError_WhenScopeIsInvalid() {
    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );
    var command = AuthorizeCommand.SetPropertyBackingField(c => c.Scope, "invalid");

    var result = await sut.Handle(command);

    result.ShouldFailWith(DomainErrors.ValueObject.InvalidCodeChallengeMode);
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenAuthenticationProviderDoesNotExist() {
    var invalidIdentityProviderError = ApplicationErrors.Authentication.InvalidIdentityProvider("aad");
    _authorizationHandlerCollectionMock
      .Setup(x => x.GetProvider(It.IsAny<string>()))
      .Returns(invalidIdentityProviderError);

    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );

    var command = AuthorizeCommand.SetPropertyBackingField(c => c.IdentityProvider, "aad");

    var result = await sut.Handle(command);

    result.ShouldFailWith(invalidIdentityProviderError);
  }

  [Fact]
  public async Task Handle_CreatesAuthenticationTicketWithAuthorizationProvider() {
    var authorizationTicket = new Mock<IAuthorizationTicket>().Object;
    _authenticationProviderMock
      .Setup(x => x.CreateAuthorizationTicketAsync(It.IsAny<AuthorizationRequest>(), default))
      .ReturnsAsync(Result.Success(authorizationTicket));

    var sut = new AuthorizationRequestCommandHandler(
      _authorizationHandlerCollectionMock.Object
    );

    var command = AuthorizeCommand;

    var result = await sut.Handle(command);

    result.ShouldSucceedWithEquivalent(authorizationTicket);
  }
}