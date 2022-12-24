using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ozone.Common.Api.Abstractions;
using Ozone.Identity.Api.Token.Contracts;
using Ozone.Identity.Core.Token.Commands;

namespace Ozone.Identity.Api.Token;

[Route("api/v1/token")]
public sealed class TokenController : ApiController {
  public TokenController(IMediator handler, IMapper mapper) : base(handler, mapper) { }

  [HttpPost]
  public async Task<IActionResult> GrantToken(
    // We use a custom model binder (TokenGrantRequestModelBinder)
    // as the body of the request can only be read once. If it is read
    // by the default model binder, we would not be able to read it again
    // in order to get access to grant type specific properties.
    [FromForm] TokenGrantRequest grantRequest
  ) {
    var grantCommand = Mapper.Map<TokenGrantRequest, TokenGrantCommand>(grantRequest);
    var grantResult = await Handler.Send(grantCommand);

    return grantResult.IsFailure
      ? HandleBadRequest(grantResult.Error)
      : Ok(grantResult.Value);
  }
}