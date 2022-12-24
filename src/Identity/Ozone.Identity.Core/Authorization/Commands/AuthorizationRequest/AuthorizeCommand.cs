using Ozone.Common.Core.Abstractions;
using Ozone.Common.Core.Messaging;
using Ozone.Identity.Core.Authorization.Provider;

namespace Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;

public sealed record AuthorizeCommand : ICommand<IAuthorizationTicket> {
  public required string ClientId { get; init; }
  public required Uri RedirectUri { get; init; }
  public required string ResponseMode { get; init; }
  public required string Scope { get; init; }
  public required string State { get; init; }
  public required string CodeChallenge { get; init; }
  public required string CodeChallengeMethod { get; init; }
  public required string? IdentityProvider { get; init; }
  public string? KeyChain { get; init; }
  public ParameterCollection? ExternalProviderParameters { get; init; }
};