using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Ozone.Identity.Api.Token.Contracts.GrantTypes;

public sealed record RefreshTokenTokenGrantRequest : TokenGrantRequest {
  [FromForm(Name = "refresh_token")]
  public string RefreshToken { get; set; }
};