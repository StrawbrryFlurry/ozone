using Ozone.Common.Functional;

namespace Ozone.Identity.Api.Authorization;

public static class AuthorizationErrors {
  public static readonly Error InternalAuthenticationNotSupported = new(
    "Identity.Authorization.InternalAuthenticationNotSupported",
    "Internal authorization is not supported at this moment"
  );

  public static Error ExternalAuthenticationFailed = new(
    "Identity.Authorization.ExternalAuthenticationFailed",
    "The authentication with the external provider failed.");

  public static Error CorrelationIdIsMissing = new(
    "Identity.Authorization.InvalidCorrelationId",
    "The correlation id specified is missing or invalid."
  );
}