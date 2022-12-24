using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Authorization.Provider;

public interface IAuthenticationProviderCollection {
  public Result<IAuthenticationProvider<IAuthenticationProviderOptions>> GetProvider(string? name);
}