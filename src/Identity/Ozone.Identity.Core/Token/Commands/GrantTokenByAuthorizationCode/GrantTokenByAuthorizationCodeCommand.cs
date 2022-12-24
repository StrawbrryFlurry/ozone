using Ozone.Common.Core.Messaging;

namespace Ozone.Identity.Core.Token.Commands.GrantTokenByAuthorizationCode;

public sealed record GrantTokenByAuthorizationCodeCommand : TokenGrantCommand {
  public string Code { get; init; }
  public string ClientId { get; init; }
  public string RedirectUri { get; init; }
  public string? ClientSecret { get; init; }
  public string? CodeVerifier { get; init; }
}