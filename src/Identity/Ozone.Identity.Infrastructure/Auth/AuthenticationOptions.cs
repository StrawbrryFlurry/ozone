using Ozone.Identity.Core.Authorization;
using Ozone.Identity.Core.Authorization.Provider;

namespace Ozone.Identity.Infrastructure.Auth;

public sealed class AuthenticationOptions {
  private Dictionary<string, Type> _providers = new();
  public IReadOnlyDictionary<string, Type> Providers => _providers;

  public void AddProvider<TProvider, TOptions>(string scheme) where TProvider : IAuthenticationProvider<TOptions>
    where TOptions : IAuthenticationProviderOptions {
    _providers.Add(scheme, typeof(TProvider));
  }
}