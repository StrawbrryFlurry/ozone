using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ozone.Common.Domain.Data;
using Ozone.Common.Persistence.DependencyInjection;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Persistence.Auth.Repositories;
using Ozone.Identity.Persistence.EnterpriseApplications.Repositories;
using Ozone.Identity.Persistence.Identity.Repositories;

namespace Ozone.Identity.Persistence;

public static class DependencyInjection {
  public static IServiceCollection AddPersistence(
    this IServiceCollection services,
    Action<IServiceProvider, DbContextOptionsBuilder> dbContextBuilder
  ) {
    services.AddDbContext<IdentityContext>(dbContextBuilder);

    services.AddScoped<IUnitOfWork, IdentityContext>();

    services.AddRepository<IExternalAuthenticationChallengeRepository, ExternalAuthenticationChallengeRepository>();
    services.AddRepository<IServiceApplicationRepository, ServiceApplicationRepository>();
    services.AddRepository<IAuthorizationGrantRepository, AuthorizationGrantRepository>();
    services.AddRepository<IOzoneUserRepository, OzoneUserRepository>();

    return services;
  }
}