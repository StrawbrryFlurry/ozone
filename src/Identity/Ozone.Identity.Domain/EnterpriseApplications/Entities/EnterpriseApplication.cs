using Ozone.Common.Domain.Data;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.EnterpriseApplications;

public sealed class EnterpriseApplication : Entity, IAggregateRoot {
  private readonly List<OzoneRole> _roles = new();

  public string Name { get; }
  public string Description { get; }
  public ServiceApplication ServiceApplication { get; }

  /// <summary>
  /// Authorization roles that belong
  /// to this application
  /// </summary>
  public IReadOnlyList<OzoneRole> Roles => _roles;

  [PersistenceConstructor]
  private EnterpriseApplication() { }

  private EnterpriseApplication(string name, string description) {
    Name = name;
    Description = description;
    ServiceApplication = ServiceApplication.Create(name, description, name);
  }

  public static EnterpriseApplication Create(string name, string description) {
    var application = new EnterpriseApplication(name, description);
    CreateDefaultRoles(application);
    CreateDefaultServiceActions(application.ServiceApplication);
    return application;
  }

  private static void CreateDefaultRoles(EnterpriseApplication application) {
    application.CreateRole("Application Administrator", "");
  }

  private OzoneRole CreateRole(string name, string description) {
    var role = OzoneRole.Create(this, name, description);
    _roles.Add(role);
    return role;
  }

  private static void CreateDefaultServiceActions(ServiceApplication application) {
    application.CreateServiceAction("admin", "Application Administrator", "");
  }
}