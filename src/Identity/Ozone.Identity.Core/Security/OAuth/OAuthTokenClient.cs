using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Security.OAuth;

public sealed class OAuthTokenClient : IOAuthTokenClient {
  private readonly HttpClient _httpClient;

  public OAuthTokenClient(HttpClient httpClient) {
    _httpClient = httpClient;
  }

  public async Task<Result<OAuthTokenResponse>> GetTokenByCodeAsync(OAuthCodeTokenRequest request) {
    var content = MakeTokenRequestContent(request);
    var response = await _httpClient.PostAsync(request.IdpTokenEndpoint, content);
    var responseContent = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode) {
      return ApplicationErrors.Authorization.ExternalIdentityProviderError(responseContent);
    }

    return JsonConvert.DeserializeObject<OAuthTokenResponse>(responseContent);
  }

  private FormUrlEncodedContent MakeTokenRequestContent(OAuthCodeTokenRequest request) {
    var content = new Dictionary<string, string> {
      { "client_id", request.ClientId },
      { "client_secret", request.ClientSecret },
      { "code", request.Code },
      { "grant_type", "authorization_code" },
      { "redirect_uri", request.RedirectUri }
    };
    if (request.CodeVerifier is not null) {
      content.Add("code_verifier", request.CodeVerifier);
    }

    return new FormUrlEncodedContent(content);
  }
}