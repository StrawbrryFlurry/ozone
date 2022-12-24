using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public sealed class OzoneRoleStub {
  public static OzoneRole Create(string name, Guid serviceId, IEnumerable<ServiceAction> actions) {
    var role = EntityMockFactory.CreateInstance<OzoneRole>();

    role.SetPropertyBackingField(x => x.Name, name);
    role.SetPropertyBackingField(x => x.Actions, actions);

    var enterpriseApplication = EntityMockFactory.CreateInstance<EnterpriseApplication>();
    var service = ServiceApplicationStub.Create(serviceId, "Role Mock Application", "");

    foreach (var serviceAction in actions) {
      serviceAction.SetPropertyBackingField(x => x.Service, service);
    }

    enterpriseApplication.SetPropertyBackingField(x => x.ServiceApplication, service);
    role.SetPropertyBackingField(x => x.OwningApplication, enterpriseApplication);

    return role;
  }
}