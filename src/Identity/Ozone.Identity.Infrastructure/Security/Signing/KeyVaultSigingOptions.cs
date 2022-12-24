using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace Ozone.Identity.Infrastructure.Security.Signing;

public sealed class KeyVaultSigingOptions {
  public string SigningKeyName { get; set; }
}