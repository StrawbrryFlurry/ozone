using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Ozone.Common.Api.Abstractions;
using Ozone.Common.Identification;
using Ozone.Identity.Api.Authorization.Contracts;
using Ozone.Identity.Core.Authorization.Commands.AuthorizationRequest;
using Ozone.Identity.Core.Authorization.Commands.ExternalAuthorizationCallback;

namespace Ozone.Identity.Api.Authorization;

[Route("/api/v1/authorize")]
public sealed class AuthorizationController : ApiController {
  public const string OzoneIdentityProvider = "ozone";

  public AuthorizationController(IMediator handler, IMapper mapper) : base(handler, mapper) { }

  [HttpGet]
  public async Task<IActionResult> Authorize([FromForm] AuthorizeRequest request, CancellationToken ct) {
    if (IsExternalIdentityProvider(request)) {
      return await HandleExternalIdentityProviderAuthorization(request, ct);
    }

    return HandleBadRequestAsRedirect(request.RedirectUri, AuthorizationErrors.InternalAuthenticationNotSupported);
  }

  private bool IsExternalIdentityProvider(AuthorizeRequest request) {
    var idp = request.IdentityProvider;

    return idp is not null and not OzoneIdentityProvider;
  }

  private async Task<IActionResult> HandleExternalIdentityProviderAuthorization(
    AuthorizeRequest request,
    CancellationToken ct
  ) {
    var externalAuthorizationcCommand = Mapper.Map<AuthorizeCommand>(request);
    var authorizationTicketResult = await Handler.Send(externalAuthorizationcCommand, ct);

    if (authorizationTicketResult.IsFailure) {
      return HandleBadRequestAsRedirect(request.RedirectUri, authorizationTicketResult.Error);
    }

    var ticket = authorizationTicketResult.Value;
    await ticket.InvokeAsync(HttpContext);
    return Empty;
  }

  [HttpGet]
  [Route("callback/{identityProvider}")]
  public async Task<IActionResult> ExternalAuthorizeCallback(
    string identityProvider,
    ExternalAuthenticationCallback callback,
    CancellationToken ct
  ) {
    if (callback.State is null) {
      return HandleBadRequest(AuthorizationErrors.CorrelationIdIsMissing);
    }

    var authorizationCallbackCommand = new ExternalAuthorizationCallbackCommand {
      IdentityProvider = identityProvider,
      State = callback.State,
      Code = callback.Code,
      Error = callback.Error,
      ErrorDescription = callback.ErrorDescription
    };

    var authorizationResult = await Handler.Send(authorizationCallbackCommand, ct);

    if (authorizationResult.IsFailure) {
      return HandleFailure(authorizationResult);
    }

    var redirectUri = authorizationResult.Value;

    return Redirect(redirectUri.ToString());
  }

  [HttpPost]
  public async Task<IActionResult> AuthorizeClient([FromBody] AuthorizationGrantConsentRequest authorization) {
    return Ok();
  }
}