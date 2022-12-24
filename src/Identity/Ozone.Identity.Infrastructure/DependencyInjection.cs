using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.Extensions.DependencyInjection;
using Ozone.Common.Time;
using Ozone.Identity.Core.Security;
using Ozone.Identity.Core.Security.OAuth;
using Ozone.Identity.Infrastructure.Security.Signing;

namespace Ozone.Identity.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services)
  {
    // Infrastructure
    services.AddSingleton<ISecurityTokenSigner, KeyVaultTokenSigner>();
    services.AddSingleton<IKeyVaultKeyProvider, KeyVaultKeyProvider>();
    services.AddSingleton((s) =>
      new KeyClient(new Uri("https://keyvault.azure.net/"), new DefaultAzureCredential()));

    // Core
    services.AddSingleton<ISystemClock, SystemClock>();
    services.AddSingleton<IOAuthTokenClient, OAuthTokenClient>();

    services.AddHttpClient();

    return services;
  }
}