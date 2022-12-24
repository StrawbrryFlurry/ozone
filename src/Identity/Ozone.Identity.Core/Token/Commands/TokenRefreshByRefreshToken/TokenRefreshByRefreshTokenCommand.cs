using Ozone.Common.Core.Messaging;

namespace Ozone.Identity.Core.Token.Commands.TokenRefreshByRefreshToken;

public sealed record TokenRefreshByRefreshTokenCommand : TokenGrantCommand {
  public string RefreshToken { get; set; }
}