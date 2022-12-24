using FluentAssertions;
using MediatR;
using Ozone.Common.Core.Abstractions;
using Ozone.Common.Testing.Api.Assertions;
using Ozone.Common.Testing.Api.Request;
using Ozone.Common.Testing.Api.Request.HttpClientExtensions;
using Ozone.Identity.Api.Authorization;
using Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Authorization.Provider.OAuth;
using Ozone.Testing.Common.Mocking;

namespace Ozone.Identity.Api.Tests.Integration.Authorization;

public sealed class AuthorizationControllerTests : ControllerTestBase {
  private static readonly Uri RedirectUri = new("https://client-app.redirect");

  private static FormEncodedContentBuilder RequestBodyBase => new() {
    { "client_id", "eb5e29dc-9d38-4140-91a4-8a9c7bd16864" },
    { "redirect_uri", RedirectUri.ToString() },
    { "response_mode", "query" },
    { "scope", "openid" },
    { "state", "1234" },
    { "code_challenge", "aabbcc" },
    { "code_challenge_method", "s256" },
    { "key_chain", "{ \"some\": [ \"keychain\" ] }" }
  };

  [Fact]
  public async Task Authorize_ReturnsError_WhenIdentityProviderIsInternalOrUnset() {
    var requestContent = RequestBodyBase;

    var response = await Client.GetAsync("/api/v1/authorize", requestContent.Build());

    response.ShouldBeErrorRedirect(RedirectUri, AuthorizationErrors.InternalAuthenticationNotSupported);
  }

  [Fact]
  public async Task Authorize_InvokesMediatorWithExternalAuthorizationRequestCommand_WhenRequiredPropertiesAreSet() {
    var requestContent = RequestBodyBase;
    requestContent.Add("identity_provider", "aad");
    requestContent.Add("external_provider_parameters", "{\"key\": \"value\"}");

    var r = await Client.GetAsync("/api/v1/authorize", requestContent.Build());

    var commandInvocation = HandlerMock
      .InvocationsOfName(nameof(IMediator.Send))
      .First(i => i.Arguments[0] is AuthorizeCommand);
    var command = commandInvocation.Arguments[0] as AuthorizeCommand;

    command.ClientId.Should().Be(requestContent["client_id"]);
    command.Scope.Should().Be(requestContent["scope"]);
    command.RedirectUri.Should().Be(requestContent["redirect_uri"]);
    command.ResponseMode.Should().Be(requestContent["response_mode"]);
    command.State.Should().Be(requestContent["state"]);
    command.CodeChallenge.Should().Be(requestContent["code_challenge"]);
    command.CodeChallengeMethod.Should().Be(requestContent["code_challenge_method"]);
    command.IdentityProvider.Should().Be(requestContent["identity_provider"]);
    command.KeyChain.Should().Be(requestContent["key_chain"]);
    command.ExternalProviderParameters.Should().BeEquivalentTo(new ParameterCollection { { "key", "value" } });
  }

  [Fact]
  public async Task Authorize_RedirectsClientToIdentityProvider_WhenAuthorizationWasSuccessful() {
    var requestContent = RequestBodyBase;

    requestContent.Add("identity_provider", "aad");

    var redirectTarget = new Uri("https://idp-redirect.io");
    SetupHandlerForCommand<
      AuthorizeCommand,
      IAuthorizationTicket
    >(
      new RedirectAuthorizationTicket(redirectTarget)
    );

    var response = await Client.GetAsync("/api/v1/authorize", requestContent.Build());

    response.ShouldBeRedirectTo(redirectTarget);
  }
}