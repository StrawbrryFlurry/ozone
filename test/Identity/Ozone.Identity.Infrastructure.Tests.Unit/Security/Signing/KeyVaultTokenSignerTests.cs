using Azure.Security.KeyVault.Keys;
using Moq;
using Azure.Security.KeyVault.Keys.Cryptography;
using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Signing;
using Ozone.Identity.Infrastructure.Security.Signing;
using Ozone.Testing.Common.FluentExtensions;
using Ozone.Testing.Common.Mocking;

namespace Ozone.Identity.Infrastructure.Tests.Unit.Security.Signing;

public sealed class KeyVaultTokenSignerTests {
  private const string CurrentKeyName = "CurrentKey";
  private static readonly string CurrentKeyVersion = Guid.NewGuid().ToString();
  private static readonly string CurrentKid = $"{CurrentKeyName}::{CurrentKeyVersion}";

  private readonly Mock<IKeyVaultKeyProvider> _keyVaultProviderMock = new();
  private readonly Dictionary<string, Mock<CryptographyClient>> _clientMocks = new();

  public KeyVaultTokenSignerTests() {
    Setup_DefaultKey();
  }

  [Fact]
  public async Task SignAsBase64Async_ReturnsFailedResult_WhenKeyCannotBeSigned() {
    var sut = new KeyVaultTokenSigner(_keyVaultProviderMock.Object);

    Setup_ClientForKid(CurrentKid, m =>
      m
        .Setup(c => c.SignAsync(It.IsAny<SignatureAlgorithm>(), It.IsAny<byte[]>(), default))
        .Throws(() => new InvalidOperationException("The Key \"testKey\" is not valid after today"))
    );

    var result = await sut.SignAsBase64Async(new SignableSecurityTokenStub(""));

    result.ShouldBeFailure();
  }

  [Fact]
  public async Task SignAsBase64Async_CallsSignAsyncWithHashedTokenOnCryptographyClient() {
    var sut = new KeyVaultTokenSigner(_keyVaultProviderMock.Object);

    Setup_ClientForKid(CurrentKid, m =>
      m
        .Setup(c => c.SignAsync(It.IsAny<SignatureAlgorithm>(), It.IsAny<byte[]>(), default))
        .ReturnsAsync(SignResultStub.Create())
    );


    await sut.SignAsBase64Async(new SignableSecurityTokenStub(""));

    GetClientForKid(CurrentKid)
      .InvocationsOfName(nameof(CryptographyClient.SignAsync))
      .Should().HaveCount(1);
  }

  [Fact]
  public async Task SignAsBase64Async_SignedTokenHashAsBase64_WhenSigningIsSuccessful() {
    var sut = new KeyVaultTokenSigner(_keyVaultProviderMock.Object);
    var testData = "Test";
    var signResult = SignResultStub.Create(new byte[256]);

    Setup_ClientForKid(CurrentKid, m =>
      m
        .Setup(c => c.SignAsync(It.IsAny<SignatureAlgorithm>(), It.IsAny<byte[]>(), default))
        .ReturnsAsync(signResult)
    );

    var result = await sut.SignAsBase64Async(new SignableSecurityTokenStub(testData));

    result.Value.Signature.Should().Be(signResult.Signature.ToBase64UrlEncoded());
  }

  [Fact]
  public async Task GetSingingKeysAsync_ReturnsAListOfAllActiveKeys() {
    var sut = new KeyVaultTokenSigner(_keyVaultProviderMock.Object);

    var key = KeyVaultKeyStub.CreateWithRsa(CurrentKeyName, CurrentKid);

    _keyVaultProviderMock
      .Setup(k => k.FormatKeyVersion(It.IsAny<KeyVaultKey>()))
      .Returns("TestKey");

    _keyVaultProviderMock
      .Setup(k => k.GetActiveKeysAsync(default))
      .ReturnsAsync(new List<KeyVaultKey> { key });

    var result = await sut.GetSingingKeysAsync();

    result.Value.Should().BeEquivalentTo(new List<ISecurityTokenSigningKey> {
      new RsaSecurityTokenSingingKey("TestKey", key.Key.ToRSA())
    });
  }


  private void Setup_DefaultKey() {
    _keyVaultProviderMock.Setup(k => k.ActiveKid).Returns(CurrentKid);
    _keyVaultProviderMock.Setup(k => k.SigningKeyName).Returns(CurrentKeyName);
    Setup_GetClientForKid(CurrentKid);

    _keyVaultProviderMock.Setup(m => m.GetSigningMetadata(It.IsAny<string>()))
      .Returns(Result.Success(new SecurityTokenSigningMetadata {
        Algorithm = SigningAlgorithm.Default,
        KeyId = CurrentKid
      }));
  }

  private void Setup_GetClientForKid(string kid) {
    _keyVaultProviderMock
      .Setup(client => client.GetClientForKid(kid))
      .Returns((string k) => MakeCryptographyClient(k));
  }

  private void Setup_ClientForKid(string kid, Action<Mock<CryptographyClient>> setup) {
    var client = _clientMocks.GetOrAddThreadUnsafe(kid, () => new Mock<CryptographyClient>());
    setup(client);
  }

  private Mock<CryptographyClient> GetClientForKid(string kid) {
    return _clientMocks[kid];
  }

  private CryptographyClient MakeCryptographyClient(string kid) {
    if (_clientMocks.TryGetValue(kid, out var m)) {
      return m.Object;
    }

    var client = new Mock<CryptographyClient>();
    _clientMocks.TryAdd(kid, client);

    return client.Object;
  }
}