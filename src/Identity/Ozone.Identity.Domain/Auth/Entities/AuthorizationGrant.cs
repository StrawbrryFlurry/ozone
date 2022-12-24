using Ozone.Common.Domain.Data;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.Auth;

public sealed class AuthorizationGrant : Entity, IAggregateRoot {
  private readonly List<ServiceAction> _actions = new();
  private readonly List<RefreshToken> _refreshTokens = new();

  public OzoneUser User { get; private set; }
  public ServiceApplication ServiceApplication { get; private set; }
  public IReadOnlyList<ServiceAction> GrantedActions => _actions;

  public bool IsRevoked { get; private set; }

  public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens;

  [PersistenceConstructor]
  private AuthorizationGrant() { }

  private AuthorizationGrant(
    OzoneUser user,
    ServiceApplication serviceApplication,
    List<ServiceAction> actions
  ) {
    User = user;
    ServiceApplication = serviceApplication;
    _actions = actions;
  }

  public static AuthorizationGrant Create(
    OzoneUser user,
    ServiceApplication serviceApplication,
    ICollection<ServiceAction> actions
  ) {
    return new AuthorizationGrant(user, serviceApplication, actions.ToList());
  }

  public void Revoke() {
    IsRevoked = true;

    foreach (var refreshToken in RefreshTokens) {
      refreshToken.Revoke();
    }
  }

  public bool HasGrantForAction(ServiceAction action) {
    return GrantedActions.Any(a => a.Id == action.Id);
  }

  public bool HasGrantedForActions(IEnumerable<ServiceAction> actions) {
    return actions.All(a => GrantedActions.Any(b => b.Id == a.Id));
  }
}