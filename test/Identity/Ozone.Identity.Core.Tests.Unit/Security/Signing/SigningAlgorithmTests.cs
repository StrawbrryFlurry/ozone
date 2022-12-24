using System.Security.Cryptography;
using Moq;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Tests.Unit.Security.Signing;

public sealed class SigningAlgorithmTests {
  [Fact]
  public void Parse_ReturnsRsaAlgorithm_WhenInputIsRSString() {
    var algorithm = SigningAlgorithm.Parse("RS256");

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.RS256);
  }

  [Fact]
  public void Parse_ReturnsECAlgorithm_WhenInputIsECString() {
    var algorithm = SigningAlgorithm.Parse("ES256");

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.Ec256);
  }

  [Fact]
  public void Parse_ReturnsRSA256Algorithm_WhenInputIsRSA256Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeRSA(256));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.RS256);
  }

  [Fact]
  public void Parse_ReturnsRSA384Algorithm_WhenInputIsRSA384Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeRSA(384));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.RS384);
  }

  [Fact]
  public void Parse_ReturnsRSA512Algorithm_WhenInputIsRSA512Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeRSA(512));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.RS512);
  }

  [Fact]
  public void Parse_ReturnsEC256Algorithm_WhenInputIsEC256Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeEC(256));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.Ec256);
  }

  [Fact]
  public void Parse_ReturnsEC384Algorithm_WhenInputIsEC384Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeEC(384));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.Ec384);
  }

  [Fact]
  public void Parse_ReturnsEC512Algorithm_WhenInputIsEC512Algorithm() {
    var algorithm = SigningAlgorithm.Parse(MakeEC(512));

    algorithm.Value.Should().BeEquivalentTo(SigningAlgorithm.Ec512);
  }

  [Fact]
  public void OpToString_ReturnsRS256String_WhenInputIsRS256Algorithm() {
    var algorithm = SigningAlgorithm.RS256;

    var result = (string)algorithm;

    result.Should().BeEquivalentTo("RS256");
  }

  private RSA MakeRSA(int size) {
    var rsa = new Mock<RSA>();
    var parameters = new RSAParameters { Modulus = new byte[size] };

    rsa.Setup(r => r.ExportParameters(false)).Returns(parameters);

    return rsa.Object;
  }

  private ECDsa MakeEC(int size) {
    var ec = new Mock<ECDsa>();
    var curveSize = size / 8;
    var parameters = new ECParameters { Q = new ECPoint() { X = new byte[curveSize] } };

    ec.Setup(r => r.ExportParameters(false)).Returns(parameters);

    return ec.Object;
  }
}