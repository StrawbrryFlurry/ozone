using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ozone.Identity.Core.Authorization;
using Ozone.Identity.Core.Authorization.Provider;

namespace Ozone.Identity.Infrastructure.Auth;

public static class AuthenticationProviderExtensions {
  public static AuthenticationBuilder AddOzoneAuthentication(this IServiceCollection services) {
    services.TryAddSingleton<AuthenticationOptions>();
    services.TryAddSingleton<IAuthenticationProviderCollection, AuthenticationProviderCollection>();
    return new AuthenticationBuilder(services);
  }
}

public sealed class AuthenticationBuilder {
  public AuthenticationBuilder(IServiceCollection services) {
    Services = services;
  }

  public IServiceCollection Services { get; }

  public AuthenticationBuilder AddProvider<TProvider, TOptions>(string scheme, IConfigurationSection options)
    where TProvider : class, IAuthenticationProvider<TOptions>
    where TOptions : class, IAuthenticationProviderOptions {
    Services.TryAddScoped<TProvider>();
    Services.Configure<TOptions>(options);

    Services
      .Configure<AuthenticationOptions>(
        o => o.AddProvider<TProvider, TOptions>(scheme)
      );

    return this;
  }
}