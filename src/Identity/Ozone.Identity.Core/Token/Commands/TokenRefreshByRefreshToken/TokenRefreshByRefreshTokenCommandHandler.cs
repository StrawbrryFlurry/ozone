using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Token.Commands.TokenRefreshByRefreshToken;

public sealed class TokenRefreshByRefreshTokenCommandHandler
  : ICommandHandler<TokenRefreshByRefreshTokenCommand, TokenGrantCommandResult> {
  public Task<IResult<TokenGrantCommandResult>> Handle(
    TokenRefreshByRefreshTokenCommand request,
    CancellationToken cancellationToken
  ) {
    throw new NotImplementedException();
  }
}