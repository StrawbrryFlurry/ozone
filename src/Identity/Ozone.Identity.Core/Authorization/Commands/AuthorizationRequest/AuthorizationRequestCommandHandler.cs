using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Common.Identification;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;

public sealed class AuthorizationRequestCommandHandler
  : ICommandHandler<AuthorizeCommand, IAuthorizationTicket> {
  private readonly IAuthenticationProviderCollection _authenticationProviders;

  public AuthorizationRequestCommandHandler(IAuthenticationProviderCollection authenticationProviders) {
    _authenticationProviders = authenticationProviders;
  }

  public async Task<IResult<IAuthorizationTicket>> Handle(
    AuthorizeCommand authorizationCommand,
    CancellationToken ct = default
  ) {
    var handlerName = authorizationCommand.IdentityProvider;
    var handlerResult = _authenticationProviders.GetProvider(handlerName);

    if (handlerResult.IsFailure) {
      return Result.Failure<IAuthorizationTicket>(handlerResult.Error);
    }

    var handler = handlerResult.Value;
    var authorizationRequestResult = MakeAuthorizationRequest(authorizationCommand, handler.IdentityProvider);

    if (authorizationRequestResult.IsFailure) {
      return Result.Failure<IAuthorizationTicket>(authorizationRequestResult.Error);
    }

    var challengeResult = await handler.CreateAuthorizationTicketAsync(authorizationRequestResult.Value, ct);

    return challengeResult;
  }

  private Result<Provider.AuthorizationRequest> MakeAuthorizationRequest(
    AuthorizeCommand request,
    string identityProvider
  ) {
    var challengeMethodResult = CodeChallengeMode.CreateFrom(request.CodeChallengeMethod);
    var responseModeResult = ResponseMode.CreateFrom(request.ResponseMode);
    var authorizationScopeResult = AuthorizationScopes.CreateFrom(request.Scope);
    var keychainResult = AuthorizationKeyChain.CreateFrom(request.KeyChain);

    if (!Guid.TryParse(request.ClientId, out var clientId)) {
      return ApplicationErrors.Authorization.InvalidClientId;
    }

    if (Result.HasFailure(
          out var error,
          challengeMethodResult,
          responseModeResult,
          authorizationScopeResult,
          keychainResult
        )
       ) {
      return error;
    }

    return new Provider.AuthorizationRequest {
      ClientId = clientId,
      ResponseMode = responseModeResult.Value,
      RedirectUri = request.RedirectUri,
      Scope = authorizationScopeResult.Value,
      State = request.State,
      CodeChallenge = request.CodeChallenge,
      CodeChallengeMethod = challengeMethodResult.Value,
      IdentityProvider = identityProvider,
      ExternalProviderParameters = request.ExternalProviderParameters,
      CorrelationId = new CorrelationId(),
      RequestedKeyChain = keychainResult.Value
    };
  }
}