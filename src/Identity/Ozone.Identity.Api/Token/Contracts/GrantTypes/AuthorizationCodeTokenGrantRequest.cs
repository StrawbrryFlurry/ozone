using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Ozone.Identity.Api.Token.Contracts.GrantTypes;

public sealed record AuthorizationCodeTokenGrantRequest : TokenGrantRequest {
  [FromForm(Name = "code")]
  public string Code { get; set; } = null!;

  [FromForm(Name = "client_id")]
  public string ClientId { get; set; } = null!;

  [FromForm(Name = "redirect_uri")]
  public string RedirectUri { get; set; } = null!;

  [FromForm(Name = "client_secret")]
  public string? ClientSecret { get; set; }

  [FromForm(Name = "code_verifier")]
  public string? CodeVerifier { get; set; }
}