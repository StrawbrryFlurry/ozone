using Ozone.Common.Functional;
using Ozone.Identity.Domain.Identity.ValueObjects;

namespace Ozone.Identity.Domain.Identity;

public interface IOzoneUserRepository {
  public Task<Result<OzoneUser>> GetUserIdentityAsync(UserIdentifier userId);
}