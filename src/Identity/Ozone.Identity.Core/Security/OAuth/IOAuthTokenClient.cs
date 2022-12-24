using Ozone.Common.Functional;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Security.OAuth;

public interface IOAuthTokenClient {
  public Task<Result<OAuthTokenResponse>> GetTokenByCodeAsync(OAuthCodeTokenRequest request);
}