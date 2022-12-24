using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Common.Time;

namespace Ozone.Identity.Domain.Auth;

public sealed class RefreshToken : Entity {
  public DateTimeOffset ExpiresAt { get; private set; }
  public string OpaqueToken { get; }
  public AuthorizationGrant Grant { get; }

  public bool Revoked { get; private set; }

  public void Revoke() {
    Revoked = true;
    ExpiresAt = DateTimeOffset.MinValue;
  }

  public bool IsExpired(ISystemClock clock) {
    if (Revoked) {
      return true;
    }

    return ExpiresAt < clock.UtcNow;
  }
}