using Ozone.Common.Functional;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Errors;

public static class ApplicationErrors {
  public static class SecurityToken {
    public static Error InvalidJwtToken(string error) {
      return new Error(
        "InvalidJwtToken", error
      );
    }
  }

  public static class Authentication {
    public static Error InvalidParameter(string parameter) {
      return new Error("Identity.InvalidAuthenticationParameter",
        $"The parameter '{parameter}' in the authentication request is invalid.");
    }

    public static Error InvalidIdentityProvider(string provider) {
      return new Error("Identity.InvalidIdentityProvider",
        $"The identity provider '{provider}' is invalid.");
    }
  }

  public static class Authorization {
    public static Error InvalidCallbackState(string state) {
      return new Error("Identity.InvalidCallbackState",
        $"The state '{state}' in the callback request is invalid.");
    }

    public static Error MultipleServiceScopesInKeyChain = new(
      "Identity.MultipleServiceScopesInKeyChain",
      "The KeyChain contains scopes from different services. Clients may only request a keychain containing scopes from a single service."
    );

    public static Error ExternalAuthenticationChallengeExpired(string correlationId) {
      return new Error(
        "ExternalAuthenticationChallengeExpired",
        $"External authentication challenge with correlation id '{correlationId}' expired."
      );
    }

    public static Error InvalidKeyChainAction(string action) {
      return new Error(
        "InvalidKeyChainAction",
        $"The requested key chain action '{action}' is invalid or does not exist."
      );
    }

    public static Error InvalidAuthorizationScope(string scope) {
      return new Error(
        "InvalidAuthorizationScope",
        $"The requested authorization scope is invalid or does not exist. Scope: '{scope}'"
      );
    }

    public static Error UnauthorizedAuthorizationScopeRequest(string scope) {
      return new Error(
        "UnauthorizedAuthorizationScopeRequest",
        $"The client is not authorized to request the scope: '{scope}'."
      );
    }

    public static Error UnauthorizedAuthorizationServiceActionsRequest(string scope) {
      return new Error(
        "UnauthorizedAuthorizationServiceActionsRequest",
        $"The client is not authorized to request the service action: '{scope}'."
      );
    }

    public static Error ExternalIdentityProviderError(string error) {
      return new Error("Identity.ExternalIdentityProviderError", error);
    }

    public static Error ExternalIdentityProviderAuthenticationFailed(string externalErrorDescription) {
      return new Error(
        "ExternalIdentityProviderAuthenticationFailed",
        externalErrorDescription
      );
    }

    public static Error ExternalIdentityProviderIdTokenInvalidNonce =
      new("ExternalIdentityProviderIdTokenInvalidNonce",
        "The nonce in the id token of the external identity provider did not contain the expected value.");

    public static Error InvalidKeyChainActionFormat(string action) {
      return new Error(
        "Identity.InvalidKeyChainActionFormat",
        $"The action '{action}' is not in a valid format.");
    }

    public static Error InvalidClientId = new(
      "Identity.InvalidClientId",
      "The client id provided is not valid");

    public static Error ExternalIdentityProviderInvalidIdToken = new(
      "ExternalIdentityProviderInvalidIdToken",
      "The external identity provider did not return an id token or it was invalid."
    );

    public static Error ExternalIdentityProviderInvalidRefreshToken = new(
      "ExternalIdentityProviderInvalidRefreshToken",
      "The external identity provider did not return a refresh token or it was invalid."
    );

    public static Error ExternalIdentityProviderAccessTokenObjectIdDidNotMatchIdToken(
      string idTokenId,
      string accessTokenId
    ) {
      return new Error(
        "ExternalIdentityProviderAccessTokenObjectIdDidNotMatchIdToken",
        "The external identity provider did not return an access token with the same object id as the id token. "
        + $"Id token object id: {idTokenId}, access token object id: {accessTokenId}"
      );
    }

    public static Error ExternalIdentityProviderInvalidAccessToken = new(
      "ExternalIdentityProviderInvalidAccessToken",
      "The external identity provider did not return an access token or it was invalid."
    );
  }
}