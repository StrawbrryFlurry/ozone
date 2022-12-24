using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Identity.ValueObjects;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Tests.Integration.Identity.Entities;

public sealed class OzoneUserTests
{
  private IdentityContext _context = IdentityContextFactory.Instance;
  private DbSet<OzoneUser> _users = IdentityContextFactory.DbSet<OzoneUser>();

  private const string IdentityProvider = "IdentityProvider";
  private const string UserId = "UserId";
  private const string Username = "michael@ozone.oi";
  private const string DisplayName = "Michael";

  [Fact]
  public async Task AddToDb_SerializesAndDeserializesUserIdentifier()
  {
    var user = OzoneUser.Create(IdentityProvider, UserId, Username, DisplayName).Value;

    await _users.AddAsync(user);
    await _context.SaveChangesAsync();

    var dbUser = await _users.FindAsync(user.Id);
    dbUser!.Id.Should().Be(UserIdentifier.Create(IdentityProvider, UserId).Value);
  }

  [Fact]
  public async Task AddToDb_SerializesUserRoles()
  {
    var user = OzoneUser.Create(IdentityProvider, UserId, Username, DisplayName).Value;
    var application = EnterpriseApplication.Create("foo", "bar");
    var role = OzoneRole.Create(application, "someRole", "Some Role");

    user.AddRole(role);

    await _users.AddAsync(user);
    await _context.SaveChangesAsync();

    var dbUser = await _users.FindAsync(user.Id);
    dbUser!.Roles.Should().ContainSingle(x => x.Name == "someRole");
  }
}