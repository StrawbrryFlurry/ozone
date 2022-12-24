using Moq;
using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public sealed class ServiceApplicationStub {
  public static ServiceApplication Create(string name, string description) {
    var application = EntityMockFactory.CreateInstance<ServiceApplication>();
    application.SetPropertyBackingField(x => x.Name, name);
    application.SetPropertyBackingField(x => x.Description, description);
    return application;
  }

  public static ServiceApplication Create(Guid id, string name, string description) {
    var application = Create(name, description);
    application.SetPropertyBackingField(x => x.Id, id);
    return application;
  }
}