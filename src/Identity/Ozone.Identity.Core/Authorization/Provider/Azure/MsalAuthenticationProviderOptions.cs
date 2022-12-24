using Ozone.Identity.Core.Authorization.Provider.OAuth;

namespace Ozone.Identity.Core.Authorization.Provider.Azure;

public sealed class MsalAuthenticationProviderOptions : OAuthAuthenticationProviderOptions {
  public string TenantId { get; set; }
}