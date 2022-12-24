using Microsoft.AspNetCore.Mvc;
using Ozone.Common.Api.Mapping.Collections;
using Ozone.Common.Core.Abstractions;

namespace Ozone.Identity.Api.Authorization.Contracts;

public sealed record AuthorizeRequest(
  [FromForm(Name = "client_id")]
  string ClientId,
  [FromForm(Name = "redirect_uri")]
  Uri RedirectUri,
  [FromForm(Name = "response_mode")]
  string ResponseMode,
  [FromForm(Name = "scope")]
  string Scope,
  [FromForm(Name = "state")]
  string State,
  [FromForm(Name = "code_challenge")]
  string CodeChallenge,
  [FromForm(Name = "code_challenge_method")]
  string CodeChallengeMethod,
  [FromForm(Name = "identity_provider")]
  string? IdentityProvider,
  [FromForm(Name = "external_provider_parameters")]
  [ModelBinder(BinderType = typeof(ParameterCollectionModelBinder))]
  ParameterCollection? ExternalProviderParameters,
  [FromForm(Name = "key_chain")]
  string? KeyChain
);