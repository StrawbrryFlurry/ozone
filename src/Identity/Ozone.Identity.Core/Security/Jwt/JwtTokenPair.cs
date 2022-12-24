namespace Ozone.Identity.Core.Security.Jwt;

public sealed class JwtTokenPair {
  public required string AccessToken { get; init; }
  public required string RefreshToken { get; init; }
  public string? IdToken { get; init; }
}