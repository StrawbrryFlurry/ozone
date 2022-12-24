using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Core.Authorization.Provider;

public abstract class RemoteAuthenticationProvider<TOptions> : IAuthenticationProvider<TOptions>
  where TOptions : IAuthenticationProviderOptions {
  private IServiceApplicationRepository _serviceApplicationRepository;

  protected RemoteAuthenticationProvider(IServiceApplicationRepository serviceApplicationRepository) {
    _serviceApplicationRepository = serviceApplicationRepository;
  }

  public abstract string IdentityProvider { get; }

  public abstract Task<Result<IAuthorizationTicket>> CreateAuthorizationTicketAsync(
    AuthorizationRequest request,
    CancellationToken cancellationToken
  );

  public abstract Task<Result<IAuthorizationTicket>> HandleAuthenticationCallbackAsync(
    ExternalAuthenticationCallback callback,
    CancellationToken ct
  );

  protected async Task<Result<IEnumerable<Scope>>> GetValidScopes(
    ServiceApplication service,
    AuthorizationScopes scopes
  ) {
    var scopesResult = await _serviceApplicationRepository.GetScopesByDescriptorAsync(service, scopes);

    if (scopesResult.IsFailure) {
      return scopesResult.Error;
    }

    var foundScopes = scopesResult.Value;
    var invalidScopes = foundScopes.Where(s => !s.IsGlobalScope && s.Service != service);

    // We take the slight performance hit in the error case of possibly enumerating the
    // invalid scopes twice to avoid allocating a new list for the rare case where there
    // are invalid scopes
    // ReSharper disable PossibleMultipleEnumeration
    return invalidScopes.Any()
      ? ApplicationErrors.Authorization.InvalidAuthorizationScope(invalidScopes.JoinBy(", "))
      : Result.Success<IEnumerable<Scope>>(foundScopes);
  }

  protected async Task<Result<IEnumerable<ServiceAction>>> GetValidKeyChainServiceActions(
    ServiceApplication service,
    AuthorizationKeyChain keyChain
  ) {
    var actionsResult = await _serviceApplicationRepository.GetServiceActionsByDescriptorAsync(service, keyChain);

    if (actionsResult.IsFailure) {
      return actionsResult.Error;
    }

    var foundActions = actionsResult.Value;
    var invalidActions = foundActions.Where(a => a.Service != service);

    return invalidActions.Any()
      ? ApplicationErrors.Authorization.InvalidKeyChainAction(invalidActions.JoinBy(", "))
      : Result.Success<IEnumerable<ServiceAction>>(foundActions);
  }

  protected Result ValidateUserPermissions(
    OzoneUser user,
    IEnumerable<Scope> scopes,
    IEnumerable<ServiceAction> actions
  ) {
    var scopePermissionResult = user.HasPermissionFor(scopes);
    var actionPermissionResult = user.HasPermissionFor(actions);

    if (Result.HasFailure(out var missingPermissionError, scopePermissionResult, actionPermissionResult)) {
      return missingPermissionError;
    }

    return Result.Success();
  }
}