using Microsoft.EntityFrameworkCore;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.EnterpriseApplications.Repositories;

public sealed class ServiceApplicationRepository : IServiceApplicationRepository {
  private readonly IdentityContext _context;

  public ServiceApplicationRepository(
    IdentityContext context
  ) {
    _context = context;
  }

  public async Task<Result<IReadOnlyList<Scope>>> GetScopesByDescriptorAsync(
    ServiceApplication service,
    IEnumerable<IdentityDescriptor> scopeNames
  ) {
    var identityDescriptors = scopeNames.DefineNamespaceIfNotSet(service.ServiceNamespace);
    var foundScopes = await _context.Set<Scope>().Where(sa => identityDescriptors.Contains(sa.ScopeDescriptor))
      .ToListAsync();

    if (foundScopes.Count == identityDescriptors.Count) {
      return foundScopes;
    }

    var missingScopes = identityDescriptors.Except(foundScopes.Select(s => s.ScopeDescriptor));
    return DomainErrors.Security.ScopeInvalidOrNotFound(missingScopes.JoinBy(", "));
  }

  public async Task<Result<IReadOnlyList<ServiceAction>>> GetServiceActionsByDescriptorAsync(
    ServiceApplication service,
    IEnumerable<IdentityDescriptor> serviceActions
  ) {
    var identityDescriptors = serviceActions.DefineNamespaceIfNotSet(service.ServiceNamespace);
    var foundActions = await _context.Set<ServiceAction>()
      .Where(sa => identityDescriptors.Contains(sa.ActionDescriptor))
      .ToListAsync();

    if (foundActions.Count == identityDescriptors.Count) {
      return foundActions;
    }

    var missingScopes = identityDescriptors.Except(foundActions.Select(s => s.ActionDescriptor));
    return DomainErrors.Security.ServiceActionInvalidOrNotFound(missingScopes.JoinBy(", "));
  }

  public async Task<Result<ServiceApplication>> GetServiceApplicationByIdAsync(Guid id) {
    var service = await _context.Set<ServiceApplication>().SingleOrDefaultAsync(s => s.Id == id);

    if (service is null) {
      return DomainErrors.ServiceApplication.ServiceApplicationNotFound(id);
    }

    return service;
  }
}