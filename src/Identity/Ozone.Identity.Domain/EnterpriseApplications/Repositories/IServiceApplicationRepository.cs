using Ozone.Common.Domain.Identity;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Domain.EnterpriseApplications;

public interface IServiceApplicationRepository {
  public Task<Result<IReadOnlyList<Scope>>> GetScopesByDescriptorAsync(
    ServiceApplication service,
    IEnumerable<IdentityDescriptor> scopeNames
  );

  public Task<Result<IReadOnlyList<ServiceAction>>> GetServiceActionsByDescriptorAsync(
    ServiceApplication service,
    IEnumerable<IdentityDescriptor> serviceActions
  );

  public Task<Result<ServiceApplication>> GetServiceApplicationByIdAsync(Guid id);
}