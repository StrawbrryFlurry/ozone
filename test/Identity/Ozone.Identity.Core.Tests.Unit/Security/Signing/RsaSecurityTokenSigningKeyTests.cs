using System.Security.Cryptography;
using Newtonsoft.Json;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Tests.Unit.Security.Signing;

public sealed class RsaSecurityTokenSigningKeyTests
{
  private const string SigningKey = """
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArfhM6hnhdVX+oSu1MYyK
OLbfWZCuE4KmbnJYIqZaxbe9LSo0GcFEeJmciMk+8IPF4rGOJZF1VRJnIfYbqJKi
kWqYmvOqnzqikz6Yf5svSzzZlGfxKWtP+bCHXlr1usV2r1f7VnVTFdFsw5T5GZty
1SiLLg4jm6shvIBNgCq0rmyfJCFuXdQE+5EdorTd0tqFOyLDAfAG+hCc7D4eilB6
zwETXTc+nHZajMqV12PjPl0IS+vTo6MXQKqPO1e5Qoi/nozdL2sQ5iEPDtvI0sPa
bUdBJJbNbZP5H0c6v7QU0tXNKfqpSgNmPZ4Eaooonpey/YZRAt6BDK5WGNZSl5Uz
MQIDAQAB
-----END PUBLIC KEY-----
""";

  private static readonly object Jwk = new
  {
    n =
      "rfhM6hnhdVX-oSu1MYyKOLbfWZCuE4KmbnJYIqZaxbe9LSo0GcFEeJmciMk-8IPF4rGOJZF1VRJnIfYbqJKikWqYmvOqnzqikz6Yf5svSzzZlGfxKWtP-bCHXlr1usV2r1f7VnVTFdFsw5T5GZty1SiLLg4jm6shvIBNgCq0rmyfJCFuXdQE-5EdorTd0tqFOyLDAfAG-hCc7D4eilB6zwETXTc-nHZajMqV12PjPl0IS-vTo6MXQKqPO1e5Qoi_nozdL2sQ5iEPDtvI0sPabUdBJJbNbZP5H0c6v7QU0tXNKfqpSgNmPZ4Eaooonpey_YZRAt6BDK5WGNZSl5UzMQ",
    e = "AQAB",
    kid = "id",
    kty = "RSA",
    use = "sig",
    alg = "RS256"
  };

  [Fact]
  public void ToJsonJwkEntry()
  {
    var rsa = GetPublicKey();
    var sut = new RsaSecurityTokenSingingKey("id", rsa);

    var entry = sut.ToJsonJwkEntry();
    entry.Should().Be(JsonConvert.SerializeObject(Jwk));
  }

  private RSA GetPublicKey()
  {
    var rsa = RSA.Create();
    rsa.ImportFromPem(SigningKey);
    return rsa;
  }
}