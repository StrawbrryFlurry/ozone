using Microsoft.EntityFrameworkCore;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Identity.ValueObjects;

namespace Ozone.Identity.Persistence.Identity.Repositories;

public sealed class OzoneUserRepository : IOzoneUserRepository {
  private readonly IdentityContext _context;

  public OzoneUserRepository(IdentityContext context) {
    _context = context;
  }

  public async Task<Result<OzoneUser>> GetUserIdentityAsync(UserIdentifier userId) {
    var user = await _context.Set<OzoneUser>().FirstOrDefaultAsync(x => x.Id == userId);
    return user is null
      ? DomainErrors.UserIdentity.UserNotFound(userId.ToString())
      : user;
  }
}