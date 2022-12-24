using Microsoft.EntityFrameworkCore;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Auth.Repositories;

public sealed class AuthorizationGrantRepository : IAuthorizationGrantRepository {
  private readonly IdentityContext _context;

  public AuthorizationGrantRepository(IdentityContext context) {
    _context = context;
  }

  public async Task AddGrantAsync(OzoneUser user, ServiceApplication application, ICollection<ServiceAction> actions) {
    var grant = AuthorizationGrant.Create(user, application, actions);
    await _context.Set<AuthorizationGrant>().AddAsync(grant);
  }

  public async Task<Result<AuthorizationGrant>> GetGrantAsync(OzoneUser user, ServiceApplication application) {
    var grant = await _context
      .Set<AuthorizationGrant>()
      .Where(x => x.User == user && x.ServiceApplication == application)
      .FirstOrDefaultAsync();

    return grant is null
      ? grant
      : DomainErrors.Auth.InvalidAuthorizationGrant(user.Username, application.Name);
  }

  public async Task AddAuthorizationCodeAsync(AuthorizationCode code) {
    await _context.Set<AuthorizationCode>().AddAsync(code);
  }

  public async Task AddRefreshTokenAsync(RefreshToken token) {
    await _context.Set<RefreshToken>().AddAsync(token);
  }

  public async Task<Result<AuthorizationCode>> GetAuthorizationCodeAsync(string code) {
    var authCode = await _context.Set<AuthorizationCode>().FirstOrDefaultAsync(x => x.Code == code);

    return authCode is null
      ? authCode
      : DomainErrors.Auth.InvalidAuthorizationCode(code);
  }
}