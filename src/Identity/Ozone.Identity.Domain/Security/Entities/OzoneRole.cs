using Ozone.Common.Domain.Data;
using Ozone.Identity.Domain.EnterpriseApplications;

namespace Ozone.Identity.Domain.Security;

public sealed class OzoneRole : Entity {
  private readonly List<ServiceAction> _actions = new();
  private const string RoleServiceActionPrefix = "Role: ";

  public EnterpriseApplication OwningApplication { get; private set; }
  public string Name { get; private set; }
  public string Description { get; private set; }

  /// <summary>
  /// The actions that will be granted
  /// to users with this role.
  /// </summary>
  public IReadOnlyList<ServiceAction> Actions => _actions;

  /// <summary>
  /// The service action that includes all permissions
  /// granted by this role.
  /// </summary>
  public ServiceAction RoleServiceAction { get; private set; }

  [PersistenceConstructor]
  private OzoneRole() { }

  private OzoneRole(EnterpriseApplication application, string name, string description) {
    OwningApplication = application;
    Name = name;
    Description = description;
  }

  public static OzoneRole Create(
    EnterpriseApplication application,
    string name,
    string description
  ) {
    var role = new OzoneRole(application, name, description);
    var displayName = $"{RoleServiceActionPrefix}{name}";
    role.RoleServiceAction = application.ServiceApplication.CreateServiceAction(name, displayName, description);
    return role;
  }

  public bool CanGrant(ServiceAction action) {
    return _actions.Any(a => a.Id == action.Id);
  }
}