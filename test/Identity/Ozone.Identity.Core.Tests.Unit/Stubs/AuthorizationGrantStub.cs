using Ozone.Common.Testing.Domain.Entity;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.Security;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Identity.Core.Tests.Unit.Stubs;

public sealed class AuthorizationGrantStub {
  public static AuthorizationGrant Create(
    List<ServiceAction> actions
  ) {
    var grant = EntityMockFactory.CreateInstance<AuthorizationGrant>();
    grant.SetPropertyBackingField(x => x.GrantedActions, actions);

    return grant;
  }
}