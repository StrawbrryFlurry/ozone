using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.EnterpriseApplications;

public sealed class ServiceApplication : Entity, IAggregateRoot {
  private readonly List<Scope> _remoteScopes = new();
  private readonly List<ServiceAction> _actions = new();
  private readonly List<Scope> _scopes = new();

  private List<ServiceAction>? _requestableActions;
  private List<Scope>? _requestableScopes;

  public string Name { get; private set; }
  public string Description { get; private set; }

  /// <summary>
  /// A unique, human-readable identifier for the application.
  /// </summary>
  public string ServiceNamespace { get; private set; }

  /// <summary>
  /// The scopes in this application
  /// that users can grant
  /// </summary>
  public IReadOnlyList<Scope> Scopes => _scopes;

  /// <summary>
  /// Actions that were registered by this application
  /// </summary>
  public IReadOnlyList<ServiceAction> Actions => _actions;

  /// <summary>
  /// The scopes that were granted to this application.
  /// Usually, this is done for on behalf of token requests.
  /// </summary>
  public IReadOnlyList<Scope> RemoteScopes => _remoteScopes;

  /// <summary>
  /// Scopes that a a client of this application can request
  /// </summary>
  private IReadOnlyList<Scope> RequestableScopes => _requestableScopes ??= GetRequestableScopes();

  /// <summary>
  /// Service actions that a client of this application can request
  /// </summary>
  private IReadOnlyList<ServiceAction> RequestableActions => _requestableActions ??= GetRequestableActions();

  [PersistenceConstructor]
  private ServiceApplication() { }

  private ServiceApplication(string name, string description, string serviceNamespace) {
    Name = name;
    Description = description;
    ServiceNamespace = serviceNamespace;
  }

  public static ServiceApplication Create(string name, string description, string? serviceNamespace) {
    serviceNamespace ??= IdentityDescriptor.FormatIdentityDescriptorSegment(name);
    var service = new ServiceApplication(name, description, serviceNamespace);

    return service;
  }

  public Scope CreateScope(string name, string displayName, string description) {
    var scope = Scope.Create(this, name, displayName, description);
    _scopes.Add(scope);
    return scope;
  }

  public ServiceAction CreateServiceAction(string name, string displayName, string description) {
    var action = ServiceAction.Create(this, name, displayName, description);
    _actions.Add(action);
    return action;
  }

  public void UpdateServiceNamespace(string serviceNamespace) {
    ServiceNamespace = IdentityDescriptor.FormatIdentityDescriptorSegment(serviceNamespace);
    UpdateScopeDescriptors();
    UpdateActionDescriptors();
  }

  private void UpdateScopeDescriptors() {
    foreach (var scope in Scopes) {
      scope.UpdateNamespace(ServiceNamespace);
    }
  }

  private void UpdateActionDescriptors() {
    foreach (var action in Actions) {
      action.UpdateNamespace(ServiceNamespace);
    }
  }

  public bool IsAllowedToRequestScope(Scope scope) {
    if (scope.IsGlobalScope) {
      return true;
    }

    return RequestableScopes.Any(s => s.Id == scope.Id);
  }

  public bool IsAllowedToRequestAction(ServiceAction action) {
    return RequestableActions.Any(a => a.Id == action.Id);
  }

  private List<Scope> GetRequestableScopes() {
    return _scopes.Union(_remoteScopes).ToList();
  }

  private List<ServiceAction> GetRequestableActions() {
    return _actions.Union(_remoteScopes.SelectMany(x => x.ServiceActions)).ToList();
  }
}