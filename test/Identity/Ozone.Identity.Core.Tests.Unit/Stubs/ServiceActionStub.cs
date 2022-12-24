using System.Runtime.Serialization;
using Moq;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public sealed class ServiceActionStub {
  private static readonly Dictionary<string, Guid> ServiceActionIds = new();

  public static ServiceAction Create(string name) {
    var action = EntityMockFactory.CreateInstance<ServiceAction>();
    var descriptor = IdentityDescriptor.Create(name).Value;
    action.SetPropertyBackingField(x => x.ActionDescriptor, descriptor);
    action.SetPropertyBackingField(x => x.Name, name);
    action.SetPropertyBackingField(x => x.DisplayName, name);

    if (!ServiceActionIds.ContainsKey(name)) {
      ServiceActionIds.Add(name, action.Id);
      return action;
    }

    action.SetPropertyBackingField(x => x.Id, ServiceActionIds[name]);
    return action;
  }

  public static ServiceAction Create(string name, string description, Guid serviceId) {
    var action = Create(name, description);
    var service = ServiceApplicationStub.Create(serviceId, "Service Action Mock Application", "");
    action.SetPropertyBackingField(x => x.Service, service);
    return action;
  }

  public static ServiceAction Create(string name, string description) {
    var action = Create(name);
    action.SetPropertyBackingField(x => x.Description, description);
    return action;
  }
}