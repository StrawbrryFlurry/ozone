using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Ozone.Common.Domain.Data;

namespace Ozone.Common.Persistence.DependencyInjection;

public static class ServiceCollectionExtensions {
  public static void AddRepository<TRepository, TImplementation>(this IServiceCollection services)
    where TRepository : class
    where TImplementation : class, TRepository {
    services.AddScoped<TRepository, TImplementation>();
  }
}