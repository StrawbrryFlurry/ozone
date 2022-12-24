using System.Net.Mime;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Ozone.Common.Core.Abstractions;
using Ozone.Common.Domain.Data;
using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Common.Time;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Core.Security.Jwt;
using Ozone.Identity.Core.Security.OAuth;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity.ValueObjects;

namespace Ozone.Identity.Core.Authorization.Provider.OAuth;

public abstract class OAuthAuthenticationProvider : RemoteAuthenticationProvider<OAuthAuthenticationProviderOptions> {
  public const string AuthenticationProviderScheme = "oauth";

  private readonly ISystemClock _clock;
  private readonly IOAuthTokenClient _tokenClient;
  private readonly IOzoneUserRepository _userRepository;
  private readonly IExternalAuthenticationChallengeRepository _challengeRepository;
  private readonly IAuthorizationGrantRepository _authorizationGrantRepository;
  private readonly IServiceApplicationRepository _serviceApplicationRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly OAuthAuthenticationProviderOptions _options;
  private string? _endpointUri;
  private string? _redirectUri;

  private const int CodeChallengeLength = 64;
  private const int NonceLength = 32;

  protected OAuthAuthenticationProvider(
    OAuthAuthenticationProviderOptions options,
    ISystemClock clock,
    IOAuthTokenClient tokenClient,
    IOzoneUserRepository userRepository,
    IExternalAuthenticationChallengeRepository challengeRepository,
    IAuthorizationGrantRepository authorizationGrantRepository,
    IServiceApplicationRepository serviceApplicationRepository,
    IUnitOfWork unitOfWork
  ) : base(serviceApplicationRepository) {
    _options = options;
    _clock = clock;
    _tokenClient = tokenClient;
    _userRepository = userRepository;
    _challengeRepository = challengeRepository;
    _authorizationGrantRepository = authorizationGrantRepository;
    _serviceApplicationRepository = serviceApplicationRepository;
    _unitOfWork = unitOfWork;
  }

  private string EndpointUri {
    get { return _endpointUri ??= GetEndpointUri(); }
  }

  private string RedirectUri {
    get { return _redirectUri ??= GetRedirectUri(); }
  }

  public abstract CodeChallengeMode CodeChallengeMode { get; }

  protected virtual string GetEndpointUri() {
    return $"https://{_options.Instance}";
  }

  protected virtual string GetRedirectUri() {
    return $"{_options.AuthorizeUri}/callback/{IdentityProvider}";
  }

  public override Task<Result<IAuthorizationTicket>> CreateAuthorizationTicketAsync(
    AuthorizationRequest request,
    CancellationToken ct
  ) {
    return AssertValidParameters(request.ExternalProviderParameters)
      .Bind(() => MakeAuthenticationChallenge(request))
      .Tap(challenge => _challengeRepository.AddAsync(challenge))
      .Tap(_ => _unitOfWork.CommitAsync(ct))
      .Map(MakeAuthenticationRequestUri)
      .Map(uri => (IAuthorizationTicket)new RedirectAuthorizationTicket(uri.ToString()));
  }

  protected abstract Result AssertValidParameters(ParameterCollection? options);

  private async Task<Result<ExternalAuthenticationChallenge>> MakeAuthenticationChallenge(
    AuthorizationRequest request
  ) {
    var correlationId = request.CorrelationId;
    var clientApplicationResult = await _serviceApplicationRepository.GetServiceApplicationByIdAsync(request.ClientId);

    if (clientApplicationResult.IsFailure) {
      return clientApplicationResult.Error;
    }

    var clientApplication = clientApplicationResult.Value;
    var scopesResult = await GetValidScopes(clientApplication, request.Scope);
    var keyChainResult = await GetValidKeyChainServiceActions(clientApplication, request.RequestedKeyChain);

    if (Result.HasFailure(out var error, scopesResult, keyChainResult)) {
      return error;
    }

    return new ExternalAuthenticationChallenge(_clock) {
      ClientApplication = clientApplicationResult.Value,
      IdentityProvider = request.IdentityProvider,
      State = request.State,
      CodeChallengeMode = request.CodeChallengeMethod,
      CodeChallenge = request.CodeChallenge,
      RedirectUri = request.RedirectUri,
      ResponseMode = request.ResponseMode,
      CorrelationId = correlationId,
      IdpState = correlationId,
      IdpCodeChallengeMode = CodeChallengeMode,
      IdpCodeChallenge = MakeCodeChallenge(out var codeVerifier),
      IdpCodeChallengeVerifier = codeVerifier,
      IdpNonce = MakeNonce(),
      Scopes = scopesResult.Value,
      Keychain = keyChainResult.Value
    };
  }

  /// <summary>
  /// Generate the code verifier for <a href="https://www.rfc-editor.org/rfc/rfc7636">PKCE</a>
  /// </summary>
  /// <param name="verifier">The base64 url encoded code verifier</param>
  /// <returns>The hashed code challenge</returns>
  protected virtual string MakeCodeChallenge(out string verifier) {
    verifier = RandomNumberGenerator.GetBytes(CodeChallengeLength).ToBase64UrlEncoded();
    var hashAlgorithm = CodeChallengeMode.GetHashAlgorithm();
    var hash = hashAlgorithm.ComputeHash(verifier.GetUtf8Bytes());
    return hash.ToBase64UrlEncoded();
  }

  protected string MakeNonce() {
    var nonceBytes = RandomNumberGenerator.GetBytes(NonceLength);
    return nonceBytes.ToBase64UrlEncoded();
  }

  private Uri MakeAuthenticationRequestUri(ExternalAuthenticationChallenge challenge) {
    var uriBuilder = new UriBuilder($"{EndpointUri}/authorize");

    var query = new QueryString();
    query = query.Add("client_id", _options.ClientId);
    query = query.Add("response_mode", "query");
    query = query.Add("response_type", "code id_token");
    query = query.Add("redirect_uri", RedirectUri);
    query = query.Add("scope", GetAuthorizationScopes());
    query = query.Add("state", challenge.IdpState);
    query = query.Add("nonce", challenge.IdpNonce);
    query = query.Add("code_challenge", challenge.IdpCodeChallenge);
    query = query.Add("code_challenge_method", challenge.IdpCodeChallengeMode.Value);

    uriBuilder.Query = query.ToString();

    return uriBuilder.Uri;
  }

  protected virtual string GetAuthorizationScopes() {
    return "openid";
  }

  public override async Task<Result<IAuthorizationTicket>> HandleAuthenticationCallbackAsync(
    ExternalAuthenticationCallback callback,
    CancellationToken ct
  ) {
    var challengeResult = await _challengeRepository.GetByCorrelationIdAsync(callback.CorrelationId);

    if (challengeResult.IsFailure) {
      return challengeResult.Error;
    }

    var challenge = challengeResult.Value;
    if (challenge.IsExpired(_clock)) {
      return ApplicationErrors.Authorization.ExternalAuthenticationChallengeExpired(challenge.CorrelationId);
    }

    var callbackErrorResult = HasExternalAuthenticationCallbackError(callback);

    if (callbackErrorResult.IsFailure) {
      return RedirectAuthorizationTicket.FromError(
        challenge.RedirectUri,
        challenge.State,
        callbackErrorResult.Error
      );
    }

    var authorizationResult = await HandleExternalUserAuthorization(callback, challenge);

    if (authorizationResult.IsFailure) {
      return RedirectAuthorizationTicket.FromError(
        challenge.RedirectUri,
        challenge.State,
        authorizationResult.Error
      );
    }

    return authorizationResult;
  }

  private Result HasExternalAuthenticationCallbackError(
    ExternalAuthenticationCallback callback
  ) {
    if (!IsCallbackDataError(callback)) {
      return Result.Success();
    }

    var error = callback.ErrorDescription ?? callback.Error!;
    return Result.Failure(ApplicationErrors.Authorization.ExternalIdentityProviderAuthenticationFailed(error));
  }

  private bool IsCallbackDataError(ExternalAuthenticationCallback callback) {
    if (callback.Error is not null) {
      return true;
    }

    return callback.ErrorDescription is not null;
  }

  private async Task<Result<IAuthorizationTicket>> HandleExternalUserAuthorization(
    ExternalAuthenticationCallback callback,
    ExternalAuthenticationChallenge challenge
  ) {
    var idTokenResult = GetIdTokenFromChallenge(callback, challenge);
    if (idTokenResult.IsFailure) {
      return idTokenResult.Error;
    }

    var userResult = await GetUserFromIdToken(idTokenResult.Value);
    if (userResult.IsFailure) {
      return userResult.Error;
    }

    var user = userResult.Value;
    var permissionsResult = ValidateUserPermissions(user, challenge.Scopes, challenge.Keychain);

    if (permissionsResult.IsFailure) {
      return permissionsResult.Error;
    }

    return await CreateExternalAuthorizationSuccessTicket(user, callback, challenge);
  }

  private Result<JwtSecurityToken> GetIdTokenFromChallenge(
    ExternalAuthenticationCallback callback,
    ExternalAuthenticationChallenge challenge
  ) {
    var idTokenJwtResult = JwtSecurityToken.Parse(callback.IdToken!);

    if (idTokenJwtResult.IsFailure) {
      return ApplicationErrors.Authorization.ExternalIdentityProviderInvalidIdToken;
    }

    var idTokenJwt = idTokenJwtResult.Value;
    var hasMatchingNonce = challenge.IdpNonce == idTokenJwt.Payload.Nonce;

    if (!hasMatchingNonce) {
      return ApplicationErrors.Authorization.ExternalIdentityProviderIdTokenInvalidNonce;
    }

    return idTokenJwt;
  }

  private async Task<Result<OzoneUser>> GetUserFromIdToken(JwtSecurityToken idToken) {
    var identifierResult = UserIdentifier.Create(IdentityProvider, idToken.Payload.ObjectId);

    if (identifierResult.IsFailure) {
      return identifierResult.Error;
    }

    var identityResult = await _userRepository.GetUserIdentityAsync(identifierResult.Value);
    return identityResult;
  }

  private async Task<Result<IAuthorizationTicket>> CreateExternalAuthorizationSuccessTicket(
    OzoneUser user,
    ExternalAuthenticationCallback callback,
    ExternalAuthenticationChallenge challenge
  ) {
    var tokenResult = await GetTokenByCodeAsync(callback, challenge);
    var validTokenResult = ValidateExternalAuthenticationTokenPair(tokenResult, user);

    if (validTokenResult.IsFailure) {
      return validTokenResult.Error;
    }

    var tokenPair = tokenResult.Value;
    var code = new AuthorizationCode(_clock) {
      ClientApplication = challenge.ClientApplication,
      Identity = user,
      Scopes = challenge.Scopes,
      KeyChain = challenge.Keychain,
      IdpRefreshToken = tokenPair.RefreshToken,
      CodeChallenge = challenge.CodeChallenge,
      CorrelationId = challenge.CorrelationId,
      IdentityProvider = challenge.IdentityProvider,
      RedirectUri = challenge.RedirectUri,
      CodeChallengeMode = challenge.CodeChallengeMode
    };

    await _authorizationGrantRepository.AddAuthorizationCodeAsync(code);
    await _unitOfWork.CommitAsync();

    var authorizationGrantResult = await _authorizationGrantRepository.GetGrantAsync(user, challenge.ClientApplication);

    if (authorizationGrantResult.IsFailure) {
      return new RedirectAuthorizationTicket(MakeOzoneAuthorizationCodeUri(code, challenge.State));
    }

    var grant = authorizationGrantResult.Value;

    var requestedActions = challenge.Scopes.SelectMany(scope => scope.ServiceActions).Union(challenge.Keychain);
    if (grant.HasGrantedForActions(requestedActions)) {
      return new RedirectAuthorizationTicket(MakeAuthorizedCodeUir(code, challenge.State));
    }

    return new RedirectAuthorizationTicket(MakeOzoneAuthorizationCodeUri(code, challenge.State));
  }

  private Result ValidateExternalAuthenticationTokenPair(Result<OAuthTokenResponse> tokenResult, OzoneUser user) {
    if (tokenResult.IsFailure) {
      return tokenResult.Error;
    }

    var tokenPair = tokenResult.Value;
    if (tokenPair.RefreshToken is null) {
      return ApplicationErrors.Authorization.ExternalIdentityProviderInvalidRefreshToken;
    }

    var accessToken = JwtSecurityToken.Parse(tokenPair.AccessToken);

    if (accessToken.IsFailure) {
      return ApplicationErrors.Authorization.ExternalIdentityProviderInvalidAccessToken;
    }

    var accessTokenUserId = accessToken.Value.Payload.ObjectId;
    // We don't validate the signature of the id token as we only use it to identify the user.
    // Therefore, we need to make sure that the id token contained the user id from the user.
    var idTokenHasHasSameUserAsAccessToken = accessTokenUserId == user.Id.UserId;

    return !idTokenHasHasSameUserAsAccessToken
      ? ApplicationErrors.Authorization.ExternalIdentityProviderAccessTokenObjectIdDidNotMatchIdToken(
        user.Id.UserId,
        accessTokenUserId
      )
      : Result.Success();
  }

  private Uri MakeAuthorizedCodeUir(AuthorizationCode code, string state) {
    var uriBuilder = new UriBuilder(code.RedirectUri);
    var query = new QueryString();

    query = query.Add("code", code.Code);
    query = query.Add("state", state);

    uriBuilder.Query = query.ToString();
    return uriBuilder.Uri;
  }

  private Uri MakeOzoneAuthorizationCodeUri(AuthorizationCode code, string state) {
    var uriBuilder = new UriBuilder(_options.OzoneAuthroizeUri);
    var query = new QueryString();

    var userGrantActions = code.KeyChain.Select(OAuthActionGrant.FromServiceAction);
    var userGrantScopes = code.Scopes.Select(OAuthActionGrant.FromScope);

    var combined = userGrantActions.Concat(userGrantScopes).ToList();
    var serialized = JsonConvert.SerializeObject(combined);

    query = query.Add("client_name", code.ClientApplication.Name);
    query = query.Add("client_description", code.ClientApplication.Description);
    query = query.Add("username", code.Identity.Username);
    query = query.Add("code", code.Code);
    query = query.Add("scopes", serialized);
    query = query.Add("redirect_uri", code.RedirectUri.ToString());
    query = query.Add("state", state);

    uriBuilder.Query = query.ToString();
    return uriBuilder.Uri;
  }

  private Task<Result<OAuthTokenResponse>> GetTokenByCodeAsync(
    ExternalAuthenticationCallback callback,
    ExternalAuthenticationChallenge challenge
  ) {
    var tokenEndpoint = $"{EndpointUri}/token";
    var request = new OAuthCodeTokenRequest {
      Code = callback.Code!,
      RedirectUri = RedirectUri,
      CodeVerifier = challenge.IdpCodeChallengeVerifier,
      ClientId = _options.ClientId,
      ClientSecret = _options.ClientSecret,
      IdpTokenEndpoint = tokenEndpoint
    };

    return _tokenClient.GetTokenByCodeAsync(request);
  }

  private struct OAuthActionGrant {
    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("description")]
    public string Description { get; init; }

    public static OAuthActionGrant FromServiceAction(ServiceAction action) {
      return new OAuthActionGrant {
        Name = action.DisplayName,
        Description = action.Description
      };
    }

    public static OAuthActionGrant FromScope(Scope scope) {
      return new OAuthActionGrant {
        Name = scope.DisplayName,
        Description = scope.Description
      };
    }
  }
}