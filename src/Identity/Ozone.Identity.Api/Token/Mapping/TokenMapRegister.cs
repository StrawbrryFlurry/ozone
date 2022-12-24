using Mapster;
using Ozone.Identity.Api.Token.Contracts;
using Ozone.Identity.Api.Token.Contracts.GrantTypes;
using Ozone.Identity.Core.Token.Commands;
using Ozone.Identity.Core.Token.Commands.GrantTokenByAuthorizationCode;
using Ozone.Identity.Core.Token.Commands.TokenRefreshByRefreshToken;

namespace Ozone.Identity.Api.Token.Mapping;

public sealed class TokenMapRegister : IRegister {
  public void Register(TypeAdapterConfig config) {
    config.NewConfig<TokenGrantRequest, TokenGrantCommand>()
      .Include<AuthorizationCodeTokenGrantRequest, GrantTokenByAuthorizationCodeCommand>();

    config.NewConfig<TokenGrantResponse, TokenGrantCommandResult>();
  }
}