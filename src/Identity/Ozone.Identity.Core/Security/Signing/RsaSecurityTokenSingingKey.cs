using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ozone.Identity.Core.Extensions;

namespace Ozone.Identity.Core.Security.Signing;

public sealed class RsaSecurityTokenSingingKey : ISecurityTokenSigningKey {
  public const string RsaSigningKeyType = "RSA";
  public const string RsaSigningUsage = "sig";

  public RsaSecurityTokenSingingKey(string id, RSA rsa) {
    ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

    KeyId = id;

    var parameters = rsa.ExportParameters(false);
    AssertHasModulusAndExponent(parameters);

    Algorithm = SigningAlgorithm.Parse(rsa).Value;

    Modulus = parameters.Modulus!.ToBase64UrlEncoded();
    Exponent = parameters.Exponent!.ToBase64UrlEncoded();
  }

  [JsonProperty("n")]
  public string Modulus { get; }

  [JsonProperty("e")]
  public string Exponent { get; }

  [JsonProperty("kid")]
  public string KeyId { get; }

  [JsonProperty("kty")]
  public string KeyType { get; } = RsaSigningKeyType;

  [JsonProperty("use")]
  public string Usage { get; } = RsaSigningUsage;

  [JsonIgnore()]
  public SigningAlgorithm Algorithm { get; }

  [JsonProperty("alg")]
  public string Alg => Algorithm.ToString();

  public string ToJsonJwkEntry() {
    return JsonConvert.SerializeObject(this);
  }

  private void AssertHasModulusAndExponent(RSAParameters parameters) {
    if (parameters.Modulus is null) {
      throw new ArgumentException("Rsa is missing required modulus parameter");
    }

    if (parameters.Exponent is null) {
      throw new ArgumentException("Rsa is missing required exponent parameter");
    }
  }
}