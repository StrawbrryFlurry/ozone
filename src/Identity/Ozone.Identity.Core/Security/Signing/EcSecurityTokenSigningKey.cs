using System.IO.Compression;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Ozone.Identity.Core.Extensions;

namespace Ozone.Identity.Core.Security.Signing;

public sealed class EcSecurityTokenSigningKey : ISecurityTokenSigningKey {
  public const string EcSigningKeyType = "EC";
  public const string EcSigningUsage = "sig";

  public EcSecurityTokenSigningKey(string keyId, ECDsa ec) {
    KeyId = keyId;

    var parameters = ec.ExportParameters(false);

    XCoordinate = parameters.Q.X!.ToBase64UrlEncoded();
    YCoordinate = parameters.Q.Y!.ToBase64UrlEncoded();

    Algorithm = SigningAlgorithm.Parse(ec).Value;
  }

  [JsonProperty("x")]
  public string XCoordinate { get; }

  [JsonProperty("y")]
  public string YCoordinate { get; }

  [JsonProperty("kid")]
  public string KeyId { get; }

  [JsonProperty("kty")]
  public string KeyType { get; } = EcSigningKeyType;

  [JsonProperty("use")]
  public string Usage { get; } = EcSigningUsage;

  [JsonIgnore]
  public SigningAlgorithm Algorithm { get; }

  [JsonProperty("alg")]
  public string Alg => Algorithm.ToString();

  public string ToJsonJwkEntry() {
    return JsonConvert.SerializeObject(this);
  }
}