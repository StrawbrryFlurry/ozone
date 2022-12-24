using System.Diagnostics.CodeAnalysis;
using Ozone.Common.Domain.Data;
using Ozone.Common.Identification;
using Ozone.Common.Time;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;
using Ozone.Identity.Domain.Security.Services;

namespace Ozone.Identity.Domain.Auth;

public sealed class AuthorizationCode : Entity, IAggregateRoot {
  private const int CodeBytes = 128;

  [SetsRequiredMembers]
  [PersistenceConstructor]
  private AuthorizationCode() { }

  public AuthorizationCode(ISystemClock clock) {
    Code = GenerateCode();
    ExpiresAt = clock.UtcNow.AddMinutes(5);
  }

  public string Code { get; }
  public required Uri RedirectUri { get; init; }
  public required IEnumerable<Scope> Scopes { get; init; }
  public required IEnumerable<ServiceAction> KeyChain { get; init; }
  public required OzoneUser Identity { get; init; }
  public required ServiceApplication ClientApplication { get; init; }
  public required CorrelationId CorrelationId { get; init; }
  public required string IdentityProvider { get; init; }
  public DateTimeOffset ExpiresAt { get; }

  public string? IdpRefreshToken { get; init; }
  public string? CodeChallenge { get; init; }
  public CodeChallengeMode? CodeChallengeMode { get; init; }

  public AuthorizationGrant Grant { get; private set; }

  public bool HasExpired(ISystemClock clock) {
    return clock.UtcNow >= ExpiresAt;
  }

  public void Consent(AuthorizationGrant grant) {
    Grant = grant;
  }

  private string GenerateCode() {
    return RandomBlobGenerator.GenerateString(CodeBytes);
  }
}