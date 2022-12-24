using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Identity.ValueObjects;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public sealed class OzoneUserStub {
  public static OzoneUser Create(string userId, string username = "", string displayName = "") {
    var user = EntityMockFactory.CreateInstance<OzoneUser>();

    user.SetPropertyBackingField(x => x.Id, UserIdentifier.Create("test", userId).Value);
    user.SetPropertyBackingField(x => x.Username, username);
    user.SetPropertyBackingField(x => x.DisplayName, displayName);

    return user;
  }

  public static OzoneUser Create(string userId, string username, IEnumerable<OzoneRole> roles) {
    var user = Create(userId, username);
    user.SetPropertyBackingField(x => x.Roles, roles);
    return user;
  }

  public static OzoneUser Create(string userId, IEnumerable<ServiceAction> actions) {
    var user = Create(userId);
    user.SetPropertyBackingField(x => x.DirectlyAssignedServiceActions, actions);
    return user;
  }

  public static OzoneUser Create(string userId, IEnumerable<OzoneRole> roles, IEnumerable<ServiceAction> actions) {
    var user = Create(userId);
    user.SetPropertyBackingField(x => x.DirectlyAssignedServiceActions, actions);
    return user;
  }
}