using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Identity.Domain.EnterpriseApplications;

namespace Ozone.Identity.Domain.Security;

/// <summary>
/// Represents an action in a system. Users
/// can be granted access to actions by requesting
/// the appropriate action in their keychain.
/// </summary>
public sealed class ServiceAction : Entity {
  /// <summary>
  /// The application or service this action belongs to.
  /// </summary>
  public ServiceApplication Service { get; private set; }

  /// <summary>
  /// The name of the action.
  /// E.g. Infrastructure.Script.CreateInternalUser.Invoke
  /// </summary>
  public string Name { get; private set; }

  public string DisplayName { get; private set; }
  public string Description { get; private set; }

  /// <summary>
  /// The fully qualified name of the action.
  /// E.g. Ozone-Automation:Infrastructure.Script.CreateInternalUser.Invoke
  /// </summary>
  public IdentityDescriptor ActionDescriptor { get; private set; }

  public bool IsEnabled { get; private set; }

  [PersistenceConstructor]
  private ServiceAction() { }

  private ServiceAction(
    ServiceApplication service,
    string name,
    string displayName,
    string description,
    IdentityDescriptor actionDescriptor
  ) {
    Service = service;
    Name = name;
    DisplayName = displayName;
    Description = description;
    ActionDescriptor = actionDescriptor;
    IsEnabled = true;
  }

  public static ServiceAction Create(
    ServiceApplication service,
    string name,
    string displayName,
    string description
  ) {
    var actionDescriptor = IdentityDescriptor.Create(service.ServiceNamespace, name).Value;
    return new ServiceAction(service, name, displayName, description, actionDescriptor);
  }

  public void UpdateNamespace(string ns) {
    ActionDescriptor = IdentityDescriptor.Create(ns, Name).Value;
  }
}