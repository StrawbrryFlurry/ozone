using Newtonsoft.Json;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Tests.Unit.Security.Jwt;

public sealed class JwtPayloadTests {
  [Fact]
  public void Deserialize_ReturnsPayloadWithAllClaims() {
    var input = """
    {
      "aud": "audience",
      "rti": "asdoyüxcioamdw,md.yd,xmcjklöajsd",
      "iat": 971704812,
      "keychain": "auto"
    }
    """;

    var sut = JsonConvert.DeserializeObject<JwtPayload>(input);

    sut.Audience.Should().Be("audience");
    sut.RefreshTokenId.Should().Be("asdoyüxcioamdw,md.yd,xmcjklöajsd");
    sut.IssuedAt.Should().Be(DateTimeOffset.Parse("16/10/2000 16:00:12"));
    sut.KeyChain.Where(a => a == "auto").Should().HaveCount(1);
  }
}