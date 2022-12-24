using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Errors;

namespace Ozone.Identity.Domain.Identity.ValueObjects;

public sealed class UserIdentifier : ValueObject {
  public string UserId { get; }
  public string IdentityProvider { get; }

  private UserIdentifier(string userId, string identityProvider) {
    UserId = userId;
    IdentityProvider = identityProvider;
  }

  public static Result<UserIdentifier> CreateFrom(string identity) {
    var parts = identity.Split(':');
    return Create(parts[0], parts[1]);
  }

  public static Result<UserIdentifier> Create(string identityProvider, string userId) {
    if (string.IsNullOrWhiteSpace(identityProvider)) {
      return DomainErrors.Identity.InvalidIdentityProvider(identityProvider);
    }

    if (string.IsNullOrWhiteSpace(userId)) {
      return DomainErrors.Identity.InvalidUserIdentifier(userId);
    }

    return new UserIdentifier(userId, identityProvider);
  }

  protected override IEnumerable<object?> GetEqualityComponents() {
    yield return IdentityProvider;
    yield return UserId;
  }

  public override string ToString() {
    return $"{IdentityProvider}:{UserId}";
  }
}