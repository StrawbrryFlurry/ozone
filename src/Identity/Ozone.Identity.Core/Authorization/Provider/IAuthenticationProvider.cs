using System.Net;
using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Authorization.Provider;

public interface IAuthenticationProvider<TOptions> where TOptions : IAuthenticationProviderOptions {
  public string IdentityProvider { get; }

  public Task<Result<IAuthorizationTicket>> CreateAuthorizationTicketAsync(
    AuthorizationRequest request,
    CancellationToken cancellationToken
  );

  public Task<Result<IAuthorizationTicket>> HandleAuthenticationCallbackAsync(
    ExternalAuthenticationCallback callback,
    CancellationToken ct
  );
}