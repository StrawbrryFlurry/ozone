using System.Runtime.Serialization;
using Moq;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public static class ScopeStub {
  public static readonly Dictionary<string, Guid> ScopeIds = new();

  public static Scope Create(string descriptor, string description = "") {
    var scope = EntityMockFactory.CreateInstance<Scope>();
    scope.SetPropertyBackingField(x => x.ScopeDescriptor, IdentityDescriptor.CreateFrom(descriptor).Value);
    scope.SetPropertyBackingField(x => x.Name, descriptor);
    scope.SetPropertyBackingField(x => x.DisplayName, descriptor);
    scope.SetPropertyBackingField(x => x.Description, description);

    if (!ScopeIds.ContainsKey(descriptor)) {
      ScopeIds.Add(descriptor, scope.Id);
      return scope;
    }

    scope.SetPropertyBackingField(x => x.Id, ScopeIds[descriptor]);
    return scope;
  }

  public static Scope Create(string descriptor, string description, Guid serviceId,
    IEnumerable<ServiceAction> actions) {
    var scope = Create(descriptor, description);
    scope.SetPropertyBackingField(x => x.ServiceActions, actions);
    var service = ServiceApplicationStub.Create(serviceId, "Scope Mock Application", "");
    scope.SetPropertyBackingField(x => x.Service, service);

    foreach (var serviceAction in actions) {
      serviceAction.SetPropertyBackingField(x => x.Service, service);
    }

    return scope;
  }

  public static Scope Create(string descriptor, string description, bool global) {
    var scope = Create(descriptor);
    scope.SetPropertyBackingField(x => x.IsGlobalScope, global);
    scope.SetPropertyBackingField(x => x.Description, description);
    return scope;
  }
}