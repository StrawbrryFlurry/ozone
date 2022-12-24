using Ozone.Common.Domain.Data;
using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Identity.ValueObjects;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.Identity;

public sealed class OzoneUser : Entity<UserIdentifier>, IAggregateRoot {
  private readonly List<OzoneRole> _roles = new();
  private readonly List<ServiceAction> _directlyAssignedServiceActions = new();

  /// <summary>
  /// The identity provider from
  /// which the user can sign in.
  /// </summary>
  public string IdentityProvider { get; private set; }

  public string Username { get; private set; }
  public string? DisplayName { get; private set; }

  public IReadOnlyList<OzoneRole> Roles => _roles;

  /// <summary>
  /// A list of permissions that is granted directly to the user.
  /// </summary>
  public IReadOnlyList<ServiceAction> DirectlyAssignedServiceActions => _directlyAssignedServiceActions;

  [PersistenceConstructor]
  private OzoneUser() { }

  private OzoneUser(UserIdentifier identifier, string username, string? displayName) : base(identifier) {
    IdentityProvider = identifier.IdentityProvider;
    Username = username;
    DisplayName = displayName;
  }

  public static Result<OzoneUser> Create(string identityProvider, string userId, string username, string? displayName) {
    var identifierResult = UserIdentifier.Create(identityProvider, userId);

    if (identifierResult.IsFailure) {
      return identifierResult.Error;
    }

    return new OzoneUser(identifierResult.Value, username, displayName);
  }

  public IReadOnlyList<ServiceAction> GetKeyChainForService(Guid serviceId) {
    var keyChain = new List<ServiceAction>();
    var directlyAssignedActionsOfService = _directlyAssignedServiceActions.Where(r => r.Service.Id == serviceId);
    var serviceRoles = _roles
      .Where(r => r.OwningApplication.ServiceApplication.Id == serviceId)
      .SelectMany(r => r.Actions);

    var userActionsOfService = directlyAssignedActionsOfService.Union(serviceRoles);

    foreach (var action in userActionsOfService) {
      keyChain.Add(action);
    }

    return keyChain;
  }

  public void AddRole(OzoneRole role) {
    if (_roles.Any(r => r == role)) {
      return;
    }

    _roles.Add(role);
  }

  public void AddServiceAction(ServiceAction serviceAction) {
    if (_directlyAssignedServiceActions.Any(r => r == serviceAction)) {
      return;
    }

    _directlyAssignedServiceActions.Add(serviceAction);
  }

  public Result HasPermissionFor(IEnumerable<ServiceAction> actions) {
    var notDirectlyAssignedActions = actions.Where(a => _directlyAssignedServiceActions.All(da => da.Id != a.Id));

    var unauthorizedActions = notDirectlyAssignedActions
      .Where(action =>
        !_roles.Any(r => r.OwningApplication.ServiceApplication == action.Service && r.CanGrant(action))
      ).ToArray();

    return !unauthorizedActions.Any()
      ? Result.Success()
      : DomainErrors.UserIdentity.UnauthorizedServiceAction(
        unauthorizedActions.Select(a => a.ActionDescriptor).JoinBy(", ")
      );
  }

  public Result HasPermissionFor(ServiceAction action) {
    if (_directlyAssignedServiceActions.Any(a => a.Id == action.Id)) {
      return Result.Success();
    }

    var hasRoleForAction =
      _roles.Any(r => r.OwningApplication.ServiceApplication == action.Service && r.CanGrant(action));

    return hasRoleForAction
      ? Result.Success()
      : DomainErrors.UserIdentity.UnauthorizedServiceAction(action.ActionDescriptor);
  }

  public Result HasPermissionFor(IEnumerable<Scope> scopes) {
    foreach (var scope in scopes) {
      var hasPermissionResult = HasPermissionFor(scope);

      if (hasPermissionResult.IsFailure) {
        return hasPermissionResult;
      }
    }

    return Result.Success();
  }

  public Result HasPermissionFor(Scope scope) {
    if (scope.IsGlobalScope) {
      return Result.Success();
    }

    var hasPermissionResult = HasPermissionFor(scope.ServiceActions);

    return hasPermissionResult.IsFailure
      ? DomainErrors.UserIdentity.UnauthorizedScope(scope.ScopeDescriptor)
      : Result.Success();
  }
}