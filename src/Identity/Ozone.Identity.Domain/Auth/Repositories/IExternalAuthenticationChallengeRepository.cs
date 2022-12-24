using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Common.Identification;

namespace Ozone.Identity.Domain.Auth;

public interface IExternalAuthenticationChallengeRepository : IRepository<ExternalAuthenticationChallenge> {
  public Task AddAsync(ExternalAuthenticationChallenge challenge);
  public Task<Result<ExternalAuthenticationChallenge>> GetByCorrelationIdAsync(CorrelationId id);
}