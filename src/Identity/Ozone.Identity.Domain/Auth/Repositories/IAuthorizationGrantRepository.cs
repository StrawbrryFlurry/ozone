using System.Reflection.Metadata;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.Auth;

public interface IAuthorizationGrantRepository : IRepository<AuthorizationGrant> {
  public Task AddGrantAsync(
    OzoneUser user,
    ServiceApplication application,
    ICollection<ServiceAction> actions
  );

  public Task<Result<AuthorizationGrant>> GetGrantAsync(
    OzoneUser user,
    ServiceApplication application
  );

  public Task AddAuthorizationCodeAsync(AuthorizationCode code);

  public Task AddRefreshTokenAsync(RefreshToken code);

  public Task<Result<AuthorizationCode>> GetAuthorizationCodeAsync(string code);
}