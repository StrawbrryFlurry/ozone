using System.Security.Cryptography;
using Newtonsoft.Json;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Tests.Unit.Security.Signing;

public sealed class EcSecurityTokenSigningKeyTests {
  private const string SigningKey = """
-----BEGIN PUBLIC KEY-----
MFowFAYHKoZIzj0CAQYJKyQDAwIIAQEHA0IABIIPSszxhgDlHkI6iYr1VH2yDq0y
8PuktrUZapESxGOMkIMf7DQGIkGyB0oeK7vTAMfjjSGJ0X25M66PzQgXVdg=
-----END PUBLIC KEY-----
""";

  private static readonly object Jwk = new {
    x = "gg9KzPGGAOUeQjqJivVUfbIOrTLw-6S2tRlqkRLEY4w",
    y = "kIMf7DQGIkGyB0oeK7vTAMfjjSGJ0X25M66PzQgXVdg",
    kid = "id",
    kty = "EC",
    use = "sig",
    alg = "ES256"
  };

  [Fact]
  public void ToJsonJwkEntry() {
    var ec = GetPublicKey();
    var sut = new EcSecurityTokenSigningKey("id", ec);

    var entry = sut.ToJsonJwkEntry();

    entry.Should().Be(JsonConvert.SerializeObject(Jwk));
  }

  private ECDsa GetPublicKey() {
    var ec = ECDsa.Create();
    ;
    ec.ImportFromPem(SigningKey);
    return ec;
  }
}