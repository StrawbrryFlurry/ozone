using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Identity.Domain.EnterpriseApplications;

namespace Ozone.Identity.Domain.Security;

public sealed class Scope : Entity {
  private readonly List<ServiceAction> _serviceActions = new();

  public ServiceApplication Service { get; }

  /// <summary>
  /// The name of this scope
  /// E.g. Script.Invoke
  /// </summary>
  public string Name { get; private set; }

  public string DisplayName { get; private set; }
  public string Description { get; private set; }

  /// <summary>
  /// The fully qualified name of this scope
  /// E.g. Ozone-Automation:Script.Invoke
  /// </summary>
  public IdentityDescriptor ScopeDescriptor { get; private set; }

  /// <summary>
  /// Indicates whether the scope can be requested by any
  /// client or application. This would include scopes like
  /// standard OpenID Connect scopes. E.g. openid, profile, email, etc.
  /// If this option is not set, only clients that are explicitly
  /// allowed to request this scope can do so.
  /// </summary>
  public bool IsGlobalScope { get; private set; }

  /// <summary>
  /// A list of service actions that the scope implicitly
  /// requests access to.
  /// </summary>
  public IReadOnlyList<ServiceAction> ServiceActions => _serviceActions;

  [PersistenceConstructor]
  private Scope() { }

  private Scope(
    ServiceApplication service,
    string name,
    string displayName,
    string description,
    IdentityDescriptor scopeDescriptor
  ) {
    Service = service;
    Name = name;
    DisplayName = displayName;
    Description = description;
    ScopeDescriptor = scopeDescriptor;
  }

  public static Scope Create(ServiceApplication service, string name, string displayName, string description) {
    var descriptor = IdentityDescriptor.Create(service.ServiceNamespace, name).Value;
    return new Scope(service, name, displayName, description, descriptor);
  }

  public void UpdateNamespace(string ns) {
    ScopeDescriptor = IdentityDescriptor.Create(ns, Name).Value;
  }
}