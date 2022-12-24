using Microsoft.AspNetCore.Mvc;

namespace Ozone.Identity.Api.Authorization.Contracts;

public sealed record ExternalAuthenticationCallback(
  [FromQuery(Name = "state")]
  string? State,
  [FromQuery(Name = "code")]
  string? Code,
  [FromQuery(Name = "id_token")]
  string? IdToken,
  [FromQuery(Name = "error")]
  string? Error,
  [FromQuery(Name = "error_description")]
  string? ErrorDescription
);