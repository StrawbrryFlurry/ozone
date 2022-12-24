using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Token.Commands.GrantTokenByAuthorizationCode;

public sealed class GrantTokenByAuthorizationCodeCommandHandler
  : ICommandHandler<GrantTokenByAuthorizationCodeCommand, TokenGrantCommandResult> {
  public Task<IResult<TokenGrantCommandResult>> Handle(
    GrantTokenByAuthorizationCodeCommand request,
    CancellationToken cancellationToken
  ) {
    throw new NotImplementedException();
  }
}