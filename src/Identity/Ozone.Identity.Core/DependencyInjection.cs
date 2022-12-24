using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ozone.Identity.Core;

public static class DependencyInjection {
  public static IServiceCollection AddApplicationCore(this IServiceCollection services) {
    services.AddMediatR(AssemblyReference.Assembly);
    return services;
  }
}