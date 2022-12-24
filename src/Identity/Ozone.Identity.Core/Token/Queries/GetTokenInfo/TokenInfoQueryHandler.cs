using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Token.Queries.TokenInfo;

namespace Ozone.Identity.Core.Token.Queries.GetTokenInfo;

public sealed class TokenInfoQueryHandler : IQueryHandler<TokenInfoQuery, TokenInfoQueryResponse> {
  public Task<IResult<TokenInfoQueryResponse>> Handle(TokenInfoQuery request, CancellationToken cancellationToken) {
    throw new NotImplementedException();
  }
}