using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;
using Ozone.Common.Identification;
using Ozone.Identity.Core.Authorization.Provider;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Domain.Errors;

namespace Ozone.Identity.Core.Authorization.Commands.ExternalAuthorizationCallback;

public sealed class
  ExternalAuthorizationCallbackCommandHandler :
    ICommandHandler<ExternalAuthorizationCallbackCommand, IAuthorizationTicket> {
  private readonly IAuthenticationProviderCollection _authenticationProviders;

  public ExternalAuthorizationCallbackCommandHandler(IAuthenticationProviderCollection authenticationProviders) {
    _authenticationProviders = authenticationProviders;
  }

  public async Task<IResult<IAuthorizationTicket>> Handle(
    ExternalAuthorizationCallbackCommand request,
    CancellationToken ct
  ) {
    var authorizationHandlerResult = _authenticationProviders.GetProvider(request.IdentityProvider);

    if (authorizationHandlerResult.IsFailure) {
      return Result.Failure<IAuthorizationTicket>(authorizationHandlerResult.Error);
    }

    if (!CorrelationId.TryParse(request.State, out var correlationId)) {
      return Result.Failure<IAuthorizationTicket>(ApplicationErrors.Authorization.InvalidCallbackState(request.State));
    }

    var externalAuthorizationCallback = new ExternalAuthenticationCallback {
      Code = request.Code,
      IdToken = request.IdToken,
      CorrelationId = correlationId,
      Error = request.Error,
      ErrorDescription = request.ErrorDescription
    };

    var authorizationHandler = authorizationHandlerResult.Value;
    return await authorizationHandler.HandleAuthenticationCallbackAsync(externalAuthorizationCallback, ct);
  }
}