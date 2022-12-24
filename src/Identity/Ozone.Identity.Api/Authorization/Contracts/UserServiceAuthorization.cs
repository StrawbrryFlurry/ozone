namespace Ozone.Identity.Api.Authorization.Contracts;

public sealed record AuthorizationGrantConsentRequest {
  public string AuthorizationCode { get; init; }
}