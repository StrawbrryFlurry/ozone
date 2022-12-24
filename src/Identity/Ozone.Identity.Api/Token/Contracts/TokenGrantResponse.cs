using Newtonsoft.Json;

namespace Ozone.Identity.Api.Token.Contracts;

public sealed class TokenGrantResponse {
  [JsonProperty("access_token")]
  public string AccessToken { get; set; }

  [JsonProperty("refresh_token")]
  public string? RefreshToken { get; set; }

  [JsonProperty("token_type")]
  public string TokenType { get; set; }

  [JsonProperty("expires_in")]
  public int ExpiresIn { get; set; }
}