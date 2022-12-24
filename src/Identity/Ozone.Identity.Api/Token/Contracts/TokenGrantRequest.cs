using Microsoft.AspNetCore.Mvc;

namespace Ozone.Identity.Api.Token.Contracts;

public record TokenGrantRequest {
  public const string GrantTypeField = "grant_type";

  [FromForm(Name = GrantTypeField)]
  public string GrantType { get; set; }
};