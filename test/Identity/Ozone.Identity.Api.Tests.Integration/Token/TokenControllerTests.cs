using System.Net.Http.Json;
using FluentAssertions;
using Ozone.Common.Testing.Api.Request;
using Ozone.Identity.Api.Token.Contracts;
using Ozone.Identity.Core.Token.Commands;
using Ozone.Identity.Core.Token.Commands.GrantTokenByAuthorizationCode;

namespace Ozone.Identity.Api.Tests.Integration.Token;

public sealed class TokenControllerTests : ControllerTestBase {
  [Fact]
  public async Task GrantToken_ReturnsError_WhenGrantTypeIsMissing() {
    var grantRequest = new FormEncodedContentBuilder();
    var response = await Client.PostAsync("/api/v1/token", grantRequest.Build());

    response.Should().Be400BadRequest().And.HaveError("grant_type", "The grant_type field is required.");
  }

  [Fact]
  public async Task GrantToken_ReturnsError_WhenGrantTypeIsInvalid() {
    var grantRequest = new FormEncodedContentBuilder {
      { "grant_type", "invalid" }
    };
    var response = await Client.PostAsync("/api/v1/token", grantRequest.Build());

    response.Should().Be400BadRequest().And.HaveError("grant_type", "The grant_type field is invalid.");
  }

  [Fact]
  public async Task GrantToken_ReturnsError_WhenBodyIsMissingPropertiesForGrantType() {
    var grantRequest = new FormEncodedContentBuilder {
      { "grant_type", "code" }
    };
    var response = await Client.PostAsync("/api/v1/token", grantRequest.Build());

    response.Should().Be400BadRequest().And.HaveError("code", "The Code field is required.");
  }

  [Fact]
  public async Task GrantToken_ReturnsTokenResult_WhenRequestIsValid() {
    SetupHandlerForCommand<GrantTokenByAuthorizationCodeCommand, TokenGrantCommandResult>(
      new TokenGrantCommandResult {
        AccessToken = "accesstoken",
        ExpiresIn = 0,
        TokenType = "Bearer"
      });

    var grantRequest = new FormEncodedContentBuilder {
      { "grant_type", "code" },
      { "code", "code" },
      { "redirect_uri", "http://localhost:5000" },
      { "client_id", "client" },
      { "client_secret", "secret" },
      { "code_verifier", "verifier" }
    };

    var response = await Client.PostAsync("/api/v1/token", grantRequest.Build());

    response.Should().Be200Ok().And.BeAs(new TokenGrantResponse {
      AccessToken = "accesstoken",
      ExpiresIn = 0,
      TokenType = "Bearer"
    });
  }
}