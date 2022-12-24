using Microsoft.Extensions.Options;
using Ozone.Common.Core.Abstractions;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Common.Time;
using Ozone.Identity.Core.Authorization.Provider.OAuth;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.OAuth;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Errors;
using Ozone.Identity.Domain.Identity;

namespace Ozone.Identity.Core.Authorization.Provider.Azure;

public sealed class MsalAuthenticationProvider : OAuthAuthenticationProvider,
  IAuthenticationProvider<MsalAuthenticationProviderOptions> {
  public new const string AuthenticationProviderScheme = "msal";

  private const string EndpointPath = "oauth2/v2.0";
  private const string AuthenticationScopes = "openid offline_access User.Read";

  private static readonly string[] SupportedAuthorizationParameters = {
    "prompt",
    "login_hint",
    "domain_hint"
  };

  private readonly MsalAuthenticationProviderOptions _options;

  public override CodeChallengeMode CodeChallengeMode => CodeChallengeMode.SHA256;
  public override string IdentityProvider => AuthenticationProviderScheme;

  protected override string GetEndpointUri() {
    return $"https://{_options.Instance}/{_options.TenantId}/{EndpointPath}";
  }

  protected override Result AssertValidParameters(ParameterCollection? options) {
    if (options is null) {
      return Result.Success();
    }

    foreach (var option in options) {
      if (!SupportedAuthorizationParameters.Contains(option.Key)) {
        return ApplicationErrors.Authentication.InvalidParameter(option.Key);
      }
    }

    return Result.Success();
  }

  protected override string GetAuthorizationScopes() {
    return AuthenticationScopes;
  }

  public MsalAuthenticationProvider(
    IOptions<MsalAuthenticationProviderOptions> options,
    ISystemClock clock,
    IOAuthTokenClient tokenClient,
    IOzoneUserRepository userRepository,
    IExternalAuthenticationChallengeRepository challengeRepository,
    IAuthorizationGrantRepository authorizationGrantRepository,
    IServiceApplicationRepository serviceApplicationRepository,
    IUnitOfWork unitOfWork
  ) : base(
    options.Value,
    clock,
    tokenClient,
    userRepository,
    challengeRepository,
    authorizationGrantRepository,
    serviceApplicationRepository,
    unitOfWork
  ) {
    _options = options.Value;
  }
}