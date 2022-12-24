namespace Ozone.Identity.Core.Security.OAuth;

public sealed record OAuthCodeTokenRequest {
  public required string IdpTokenEndpoint { get; init; }
  public required string Code { get; init; }
  public required string RedirectUri { get; init; }
  public required string ClientId { get; init; }
  public required string ClientSecret { get; init; }
  public string? CodeVerifier { get; init; }
}