using Ozone.Common.Core.Abstractions;
using Ozone.Common.Identification;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Core.Authorization.Provider;

public sealed record AuthorizationRequest {
  public required Guid ClientId { get; init; }
  public required CorrelationId CorrelationId { get; init; }
  public required Uri RedirectUri { get; init; }
  public required ResponseMode ResponseMode { get; init; }
  public required AuthorizationScopes Scope { get; init; }
  public required AuthorizationKeyChain RequestedKeyChain { get; init; }
  public required string State { get; init; }
  public required string CodeChallenge { get; init; }
  public required CodeChallengeMode CodeChallengeMethod { get; init; }
  public required string IdentityProvider { get; init; }
  public ParameterCollection? ExternalProviderParameters { get; init; }
}