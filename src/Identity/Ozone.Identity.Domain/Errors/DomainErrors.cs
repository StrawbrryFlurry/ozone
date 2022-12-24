using Ozone.Common.Domain.Identity;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Domain.Errors;

public static class DomainErrors {
  public static class Identity {
    public static Error InvalidUserIdentifier(string identifier) {
      return new Error("InvalidUserIdentifier", $"The user identifier '{identifier}' is invalid.");
    }

    public static Error InvalidIdentityProvider(string provider) {
      return new Error("InvalidUserIdentifier", $"The identity provider '{provider}' is invalid or not supported.");
    }
  }

  public static class ValueObject {
    public static Error InvalidCodeChallengeMode = new(
      "InvalidCodeChallengeMode",
      "Invalid code challenge mode. Only 'S256' and 'S512' are supported."
    );

    public static Error InvalidResponseMode = new(
      "InvalidResponseMode",
      "Invalid response mode. Only 'query' and 'fragment' are supported."
    );
  }

  public static class ServiceApplication {
    public static Error ServiceApplicationNotFound(Guid id) {
      return new Error(
        "ServiceApplicationNotFound",
        $"Service application with id '{id}' was not found."
      );
    }
  }

  public static class Auth {
    public static Error ExternalAuthenticationChallengeNotFound(string correlationId) {
      return new Error(
        "ExternalAuthenticationChallengeNotFound",
        $"External authentication challenge with correlation id '{correlationId}' was not found."
      );
    }

    public static Error AuthCodeExpired = new("Identity.AuthCodeExpired", "The auth code has expired.");

    public static Error AuthCodeAlreadyUsed =
      new("Identity.AuthCodeAlreadyUsed", "The auth code has already been used.");

    public static Error AuthCodeInvalid(string property) {
      return new Error("Identity.AuthCodeInvalid", $"The property {property} is missing or invalid.");
    }

    public static Error InvalidAuthorizationGrant(string user, string application) {
      return new Error(
        "Identity.InvalidAuthorizationGrant",
        $"The authorization grant for user '{user}' and application '{application}' is invalid."
      );
    }

    public static Error InvalidAuthorizationCode(string code) {
      return new Error("Identity.InvalidAuthorizationCode", $"The authorization code '{code}' is invalid.");
    }
  }

  public static class UserIdentity {
    public static Error UnauthorizedServiceAction(string action) {
      return new Error(
        "UnauthorizedServiceAction",
        $"The user is not authorized to request the service action: '{action}'."
      );
    }

    public static Error UnauthorizedScope(string scope) {
      return new Error(
        "UnauthorizedScope",
        $"The user is not authorized to request the scope: '{scope}'."
      );
    }

    public static Error UserNotFound(string id) {
      return new Error("Identity.UserNotFound", $"The user with id '{id}' was not found.");
    }
  }

  public static class Security {
    public static Error ScopeInvalidOrNotFound(string scope) {
      return new Error(
        "ScopeInvalidOrNotFound",
        $"The scope '{scope}' is invalid or was not found.");
    }

    public static Error ServiceActionInvalidOrNotFound(string action) {
      return new Error(
        "ServiceActionInvalidOrNotFound",
        $"The action '{action}' is invalid or was not found.");
    }
  }
}