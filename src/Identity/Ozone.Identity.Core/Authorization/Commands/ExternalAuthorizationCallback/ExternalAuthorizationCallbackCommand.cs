using Ozone.Common.Core.Messaging;
using Ozone.Common.Identification;
using Ozone.Identity.Core.Authorization.Provider;

namespace Ozone.Identity.Core.Authorization.Commands.ExternalAuthorizationCallback;

public sealed record ExternalAuthorizationCallbackCommand : ICommand<IAuthorizationTicket> {
  public required string State { get; init; }
  public string? IdentityProvider { get; init; }
  public string? Code { get; init; }
  public string? IdToken { get; init; }
  public string? Error { get; init; }
  public string? ErrorDescription { get; init; }
}