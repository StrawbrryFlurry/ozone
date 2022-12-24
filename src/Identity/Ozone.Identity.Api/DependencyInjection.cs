using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Ozone.Identity.Api.Token.ModelBinding;

namespace Ozone.Identity.Api;

public static class DependencyInjection {
  public static IServiceCollection AddApi(this IServiceCollection services) {
    services
      .AddControllers(options => { options.ModelBinderProviders.Insert(0, new TokenGrantRequestModelBinderProvider()); }
      )
      .AddApplicationPart(AssemblyReference.Assembly);

    var config = TypeAdapterConfig.GlobalSettings;
    config.Scan(AssemblyReference.Assembly);

    services.AddSingleton(config);
    services.AddScoped<IMapper, ServiceMapper>();

    return services;
  }
}