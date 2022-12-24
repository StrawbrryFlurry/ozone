using Mapster;
using Ozone.Identity.Api.Authorization.Contracts;
using Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;
using Ozone.Identity.Core.Authorization.Commands.ConsentAuthorizationGrant;
using Ozone.Identity.Core.Authorization.Commands.ExternalAuthorizationCallback;
using Ozone.Identity.Core.Authorization.Provider;
using ExternalAuthenticationCallback = Ozone.Identity.Api.Authorization.Contracts.ExternalAuthenticationCallback;

namespace Ozone.Identity.Api.Authorization.Mapping;

public sealed class AuthorizationMapping : IRegister {
  public void Register(TypeAdapterConfig config) {
    config.NewConfig<AuthorizationRequest, AuthorizeCommand>();
    config.NewConfig<ExternalAuthenticationCallback, ExternalAuthorizationCallbackCommand>();
    config.NewConfig<AuthorizationGrantConsentRequest, ConsentAuthorizationGrantCommand>();
  }
}