using System.Diagnostics.CodeAnalysis;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Common.Identification;
using Ozone.Common.Time;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.Auth;

/// <summary>
///   Represents an authentication challenge on an external Identity Provider.
///   Once the user has authenticated on the external Identity Provider, we can
///   use this information to re-create the authentication context from the initial
///   request.
/// </summary>
public sealed class ExternalAuthenticationChallenge : Entity, IAggregateRoot, ICorrelatableEntity {
  [SetsRequiredMembers]
  [PersistenceConstructor]
  private ExternalAuthenticationChallenge() { }

  public ExternalAuthenticationChallenge(ISystemClock clock) {
    ExpiresAtUtc = clock.UtcNow.AddMinutes(5);
  }

  public required ServiceApplication ClientApplication { get; init; }
  public required Uri RedirectUri { get; init; }
  public required ResponseMode ResponseMode { get; init; }
  public required IEnumerable<Scope> Scopes { get; init; }
  public required IEnumerable<ServiceAction> Keychain { get; init; }
  public required string State { get; init; }
  public required string CodeChallenge { get; init; }
  public required CodeChallengeMode CodeChallengeMode { get; init; }
  public required string IdentityProvider { get; init; }
  public required string IdpCodeChallenge { get; init; }
  public required string IdpCodeChallengeVerifier { get; init; }
  public required CodeChallengeMode IdpCodeChallengeMode { get; init; }
  public required string IdpState { get; init; }
  public required string IdpNonce { get; init; }
  public DateTimeOffset ExpiresAtUtc { get; }
  public required CorrelationId CorrelationId { get; init; }

  public bool IsExpired(ISystemClock clock) {
    return clock.UtcNow > ExpiresAtUtc;
  }
}