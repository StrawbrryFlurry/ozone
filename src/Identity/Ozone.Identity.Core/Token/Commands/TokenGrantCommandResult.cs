namespace Ozone.Identity.Core.Token.Commands;

public sealed class TokenGrantCommandResult {
  public string AccessToken { get; set; }
  public string? RefreshToken { get; set; }
  public string TokenType { get; set; }
  public int ExpiresIn { get; set; }
}