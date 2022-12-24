using System.Net;
using Ozone.Common.Core.Abstractions;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using Ozone.Common.Time;
using Ozone.Identity.Core.Authorization;
using Ozone.Identity.Core.Authorization.Provider.OAuth;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.OAuth;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;

namespace Ozone.Identity.Core.Tests.Unit.Authorization.Provider.OAuth;

public sealed class OAuthAuthenticationProviderImpl : OAuthAuthenticationProvider {
  public override CodeChallengeMode CodeChallengeMode { get; } = CodeChallengeMode.SHA256;
  public override string IdentityProvider { get; } = "oauth-test";

  public static string AuthorizationScopes { get; } = "openid";

  protected override Result AssertValidParameters(ParameterCollection? options) {
    if (options is null) {
      return Result.Success();
    }

    return options.ContainsKey("fail") ? ApplicationErrors.Authentication.InvalidParameter("fail") : Result.Success();
  }

  protected override string GetAuthorizationScopes() {
    return AuthorizationScopes;
  }

  public OAuthAuthenticationProviderImpl(
    OAuthAuthenticationProviderOptions options,
    ISystemClock clock,
    IOAuthTokenClient tokenClient,
    IOzoneUserRepository userRepository,
    IExternalAuthenticationChallengeRepository challengeRepository,
    IAuthorizationGrantRepository authorizationGrantRepository,
    IServiceApplicationRepository serviceApplicationRepository,
    IUnitOfWork unitOfWork
  )
    : base(
      options,
      clock,
      tokenClient,
      userRepository,
      challengeRepository,
      authorizationGrantRepository,
      serviceApplicationRepository,
      unitOfWork
    ) { }
}