using Newtonsoft.Json;

namespace Ozone.Identity.Core.Security.OAuth;

public sealed record OAuthTokenResponse {
  [JsonProperty("access_token")]
  public required string AccessToken { get; init; }

  [JsonProperty("token_type")]
  public required string TokenType { get; init; }

  [JsonProperty("refresh_token")]
  public string? RefreshToken { get; init; }

  [JsonProperty("id_token")]
  public string? IdToken { get; set; }
}