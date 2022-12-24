using Moq;
using Ozone.Common.Functional;
using Ozone.Common.Identification;
using Ozone.Identity.Core.Authorization.Commands.ExternalAuthorizationCallback;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Authorization.Provider.OAuth;
using Ozone.Identity.Core.Errors;
using Ozone.Testing.Common.Extensions;
using Ozone.Testing.Common.FluentExtensions;
using Ozone.Testing.Common.Mocking;

namespace Ozone.Identity.Core.Tests.Unit.Authorization.Commands.ExternalAuthorizationCallback;

public sealed class ExternalAuthorizationCallbackCommandHandlerTests {
  private readonly Mock<IAuthenticationProviderCollection> _authorizationHandlerCollectionMock = new();
  private readonly Mock<IAuthenticationProvider<IAuthenticationProviderOptions>> _authenticationProviderMock = new();

  private const string CorrelationId = "eb5e29dc-9d38-4140-91a4-8a9c7bd16864";

  private static ExternalAuthorizationCallbackCommand ExternalAuthenticationCallback => new() {
    Code = "code",
    IdentityProvider = "test",
    Error = "some error",
    ErrorDescription = "some description",
    IdToken = "id token",
    State = CorrelationId
  };

  public ExternalAuthorizationCallbackCommandHandlerTests() {
    _authorizationHandlerCollectionMock
      .Setup(x => x.GetProvider("test"))
      .Returns(Result.Success(_authenticationProviderMock.Object));
    _authenticationProviderMock
      .Setup(x => x.HandleAuthenticationCallbackAsync(
        It.IsAny<ExternalAuthenticationCallback>(),
        It.IsAny<CancellationToken>())
      )
      .ReturnsAsync(new RedirectAuthorizationTicket(""));
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenAuthenticationProviderDoesNotExist() {
    var error = ApplicationErrors.Authentication.InvalidIdentityProvider("test");
    _authorizationHandlerCollectionMock
      .Setup(x => x.GetProvider("test"))
      .Returns(error);

    var sut = new ExternalAuthorizationCallbackCommandHandler(_authorizationHandlerCollectionMock.Object);

    var result = await sut.Handle(ExternalAuthenticationCallback, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenAuthenticationProviderReturnsError() {
    var error = ApplicationErrors.Authorization.InvalidClientId;

    _authenticationProviderMock
      .Setup(x => x.HandleAuthenticationCallbackAsync(
        It.IsAny<ExternalAuthenticationCallback>(),
        It.IsAny<CancellationToken>())
      )
      .ReturnsAsync(error);

    var sut = new ExternalAuthorizationCallbackCommandHandler(_authorizationHandlerCollectionMock.Object);

    var result = await sut.Handle(ExternalAuthenticationCallback, CancellationToken.None);

    result.ShouldFailWith(error);
  }

  [Fact]
  public async Task Handle_ReturnsError_WhenStateIsInvalidCorrelationId() {
    var command = ExternalAuthenticationCallback.SetPropertyBackingField(x => x.State, "invalid");
    var sut = new ExternalAuthorizationCallbackCommandHandler(_authorizationHandlerCollectionMock.Object);

    var result = await sut.Handle(command, CancellationToken.None);

    result.ShouldFailWith(ApplicationErrors.Authorization.InvalidCallbackState("invalid"));
  }

  [Fact]
  public async Task Handle_CallsAuthenticationProviderWithCallback() {
    var sut = new ExternalAuthorizationCallbackCommandHandler(_authorizationHandlerCollectionMock.Object);
    var command = ExternalAuthenticationCallback;

    await sut.Handle(command, CancellationToken.None);

    var callback = _authenticationProviderMock
      .FirstInvocationOfName(x => x.HandleAuthenticationCallbackAsync)
      .GetArgument<ExternalAuthenticationCallback>();

    callback.Code.Should().Be(command.Code);
    callback.Error.Should().Be(command.Error);
    callback.ErrorDescription.Should().Be(command.ErrorDescription);
    callback.IdToken.Should().Be(command.IdToken);
    callback.CorrelationId.Should().Be(new CorrelationId(Guid.Parse(command.State)));
  }
}