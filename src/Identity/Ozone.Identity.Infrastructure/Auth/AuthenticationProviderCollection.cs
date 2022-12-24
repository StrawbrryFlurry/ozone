using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Authorization;
using Ozone.Identity.Core.Authorization.Provider;

namespace Ozone.Identity.Infrastructure.Auth;

public sealed class AuthenticationProviderCollection : IAuthenticationProviderCollection {
  public const string DefaultAuthenticationProviderName = "default";

  private readonly IServiceProvider _serviceProvider;
  private readonly AuthenticationOptions _options;

  public AuthenticationProviderCollection(IServiceProvider serviceProvider, AuthenticationOptions options) {
    _serviceProvider = serviceProvider;
    _options = options;
  }

  public Result<IAuthenticationProvider<IAuthenticationProviderOptions>> GetProvider(string? name) {
    name ??= DefaultAuthenticationProviderName;

    if (!_options.Providers.TryGetValue(name, out var providerType)) {
      return Result<IAuthenticationProvider<IAuthenticationProviderOptions>>.Failed;
    }

    var provider = _serviceProvider.GetRequiredService(providerType)
      as IAuthenticationProvider<IAuthenticationProviderOptions>;
    return Result.Success<IAuthenticationProvider<IAuthenticationProviderOptions>>(provider);
  }
}