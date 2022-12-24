using Microsoft.EntityFrameworkCore;
using Ozone.Common.Functional;
using Ozone.Common.Identification;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.Errors;

namespace Ozone.Identity.Persistence.Auth.Repositories;

public sealed class ExternalAuthenticationChallengeRepository : IExternalAuthenticationChallengeRepository {
  private readonly IdentityContext _context;

  public ExternalAuthenticationChallengeRepository(IdentityContext context) {
    _context = context;
  }

  public async Task AddAsync(ExternalAuthenticationChallenge challenge) {
    await _context.AddAsync(challenge);
    await _context.SaveChangesAsync();
  }

  public async Task<Result<ExternalAuthenticationChallenge>> GetByCorrelationIdAsync(CorrelationId correlationId) {
    var challenge = await _context.Set<ExternalAuthenticationChallenge>()
      .SingleOrDefaultAsync(c => c.CorrelationId == correlationId);

    return challenge is not null
      ? Result.Success(challenge)
      : DomainErrors.Auth.ExternalAuthenticationChallengeNotFound(correlationId);
  }
}