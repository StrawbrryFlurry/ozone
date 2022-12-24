using System.Collections.Immutable;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Castle.DynamicProxy.Internal;
using Microsoft.Extensions.Options;
using Moq;
using Ozone.Identity.Infrastructure.Security.Signing;
using Ozone.Testing.Common.Configuration;
using Ozone.Testing.Common.Mocking;

namespace Ozone.Identity.Infrastructure.Tests.Unit.Security.Signing;

public sealed class KeyVaultKeyProviderTests {
  private const string OldKeyName = "OldKey";
  private const string CurrentKeyName = "CurrentKey";
  private const string NewKeyName = "NewKey";

  private static readonly string OldKeyVersion = Guid.NewGuid().ToString();
  private static readonly string CurrentKeyVersion = Guid.NewGuid().ToString();
  private static readonly string NewKeyVersion = Guid.NewGuid().ToString();

  private readonly Mock<KeyClient> _keyClientMock = new();

  private readonly IOptionsMonitor<KeyVaultSigingOptions> _options =
    new KeyVaultSigingOptions { SigningKeyName = CurrentKeyName }.ToOptionsMonitor();

  public KeyVaultKeyProviderTests() {
    _keyClientMock
      .Setup(client => client.GetCryptographyClient(It.IsAny<string>(), It.IsAny<string>()))
      .Returns(new Mock<CryptographyClient>().Object);
  }

  [Fact]
  public void Ctor_FetchesCurrentlyActiveKeyAndSetsItAsActiveKey() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    sut.ActiveKid.Should().Be($"{CurrentKeyName}::{CurrentKeyVersion}");
  }

  [Fact]
  public void GetClientForKid_FetchesTheKeyFromCache_WhenItWasFetchedByTheCtor() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    sut.GetClientForKid(sut.ActiveKid);

    _keyClientMock.InvocationsOfNames(
      nameof(KeyClient.GetKey),
      nameof(KeyClient.GetKeyAsync)
    ).Should().HaveCount(1);
  }

  [Fact]
  public void GetClientForKid_FetchesTheKidFromCache_WhenItWasFetchedByTheCtor() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    sut.GetClientForKid(sut.ActiveKid);

    _keyClientMock.InvocationsOfNames(
      nameof(KeyClient.GetKey),
      nameof(KeyClient.GetKeyAsync)
    ).Should().HaveCount(1);
  }

  [Fact]
  public void GetClientForKid_FetchesTheCryptographyClientFromCache_WhenItWasAlreadyCreated() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    sut.GetClientForKid(sut.ActiveKid);
    sut.GetClientForKid(sut.ActiveKid);
    sut.GetClientForKid(sut.ActiveKid);

    _keyClientMock
      .InvocationsOfName(nameof(KeyClient.GetCryptographyClient))
      .Should()
      .HaveCount(1);
  }

  [Fact]
  public void RefreshActiveCryptographyClient_FetchesTheCurrentActiveKey() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    var newKey = KeyVaultKeyStub.Create(CurrentKeyName, NewKeyVersion);
    Setup_GetKey(CurrentKeyName, null, newKey);
    Setup_GetKey(NewKeyName, NewKeyVersion);

    sut.RefreshActiveCryptographyClient();
    var newKid = sut.FormatKeyVersion(newKey);

    sut.ActiveKid.Should().Be(newKid);
  }

  [Fact]
  public void RefreshActiveCryptographyClient_FetchesTheCurrentKeyVaultKey() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    var newKey = KeyVaultKeyStub.Create(CurrentKeyName, NewKeyVersion);
    Setup_GetKey(CurrentKeyName, null, newKey);

    sut.RefreshActiveCryptographyClient();

    _keyClientMock
      .InvocationsOfNames(nameof(KeyClient.GetKey), nameof(KeyClient.GetKeyAsync))
      .Where(i => (string)i.Arguments[0] == CurrentKeyName)
      .Should()
      .HaveCount(2);
  }

  [Fact]
  public void RefreshActiveCryptographyClient_CachesTheCryptographyClient() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    var newKey = KeyVaultKeyStub.Create(CurrentKeyName, NewKeyVersion);
    Setup_GetKey(CurrentKeyName, null, newKey);

    sut.RefreshActiveCryptographyClient();

    var newKid = sut.FormatKeyVersion(newKey);
    sut.GetClientForKid(newKid);
    sut.GetClientForKid(newKid);

    _keyClientMock
      .InvocationsOfName(nameof(KeyClient.GetCryptographyClient))
      .Should()
      .HaveCount(1);
  }

  [Fact]
  public void RefreshActiveCryptographyClient_FetchesTheKeyUsingTheUpdatedName_IfItChanges() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();
    Setup_GetKey(NewKeyName);

    ChangeOptionKeyName(NewKeyName);
    sut.RefreshActiveCryptographyClient();

    _keyClientMock
      .InvocationsOfNames(nameof(KeyClient.GetKey), nameof(KeyClient.GetKeyAsync))
      .Should()
      .ContainSingle(i => (string)i.Arguments[0] == NewKeyName);
  }

  [Fact]
  public void GetSigningMetadata_ReturnsSigningFormat_WhenFormatIsRsa256() {
    Setup_GetKey(CurrentKeyName, null, KeyVaultKeyStub.CreateWithRsa(CurrentKeyName, CurrentKeyVersion));
    var sut = MakeSut();

    var metadata = sut.GetSigningMetadata(sut.ActiveKid);

    metadata.Value.Algorithm.ToString()
      .Should()
      .BeEquivalentTo(SignatureAlgorithm.RS256.ToString());
  }

  [Fact]
  public void GetSigningMetadata_ReturnsSigningFormat_WhenFormatIsRsa512() {
    Setup_GetKey(CurrentKeyName, null, KeyVaultKeyStub.CreateWithRsa(CurrentKeyName, CurrentKeyVersion, 512));
    var sut = MakeSut();

    var metadata = sut.GetSigningMetadata(sut.ActiveKid);

    metadata.Value.Algorithm.ToString()
      .Should()
      .BeEquivalentTo(SignatureAlgorithm.RS512.ToString());
  }

  [Fact(Skip = "ECDSA_P256 - PlatformNotSupported")]
  public void GetSigningMetadata_ReturnsSigningFormat_WhenFormatIsEcdsa256() {
    Setup_GetKey(CurrentKeyName, null, KeyVaultKeyStub.CreateWithEc(CurrentKeyName, CurrentKeyVersion));
    var sut = MakeSut();

    var metadata = sut.GetSigningMetadata(sut.ActiveKid);

    metadata.Value.Algorithm.ToString()
      .Should()
      .BeEquivalentTo(SignatureAlgorithm.ES256.ToString());
  }

  [Fact(Skip = "ECDSA_P521 - PlatformNotSupported")]
  public void GetSigningMetadata_ReturnsSigningFormat_WhenFormatIsEcdsa512() {
    Setup_GetKey(CurrentKeyName, null, KeyVaultKeyStub.CreateWithEc(CurrentKeyName, CurrentKeyVersion, 521));
    var sut = MakeSut();

    var metadata = sut.GetSigningMetadata(sut.ActiveKid);

    metadata.Value.Algorithm.ToString()
      .Should()
      .BeEquivalentTo(SignatureAlgorithm.ES512.ToString());
  }

  [Fact]
  public async Task GetActiveKeysAsync_ReturnsListOfAllActiveKeys_IgnoringDisabledOrExpiredKeys() {
    Setup_GetKey(CurrentKeyName);
    var sut = MakeSut();

    var keys = new List<KeyVaultKey>() {
      KeyVaultKeyStub.Create(CurrentKeyName, CurrentKeyVersion),
      KeyVaultKeyStub.Create(CurrentKeyName, OldKeyVersion),
      KeyVaultKeyStub.Create(CurrentKeyName, NewKeyVersion),
      KeyVaultKeyStub.Create(OldKeyName, CurrentKeyVersion),
      KeyVaultKeyStub.Create(OldKeyName, OldKeyVersion),
      KeyVaultKeyStub.Create(NewKeyName, OldKeyVersion),
      KeyVaultKeyStub.Create(NewKeyName, NewKeyVersion)
    };

    keys[1].SetEnabled(false);
    keys[4].SetEnabled(false);
    keys[5].SetExpiresOn(DateTimeOffset.MinValue);

    keys.ForEach(k => Setup_GetKey(k.Name, k.Properties.Version));

    _keyClientMock
      .Setup(k => k.GetPropertiesOfKeyVersionsAsync(CurrentKeyName, default))
      .Returns(ToAsyncPageable(keys.Select(k => k.Properties).Take(3)));

    _keyClientMock
      .Setup(k => k.GetPropertiesOfKeyVersionsAsync(OldKeyName, default))
      .Returns(ToAsyncPageable(keys.Select(k => k.Properties).Take(3)));

    _keyClientMock
      .Setup(k => k.GetPropertiesOfKeyVersionsAsync(OldKeyName, default))
      .Returns(ToAsyncPageable(keys.Select(k => k.Properties).Skip(3).Take(2)));

    _keyClientMock
      .Setup(k => k.GetPropertiesOfKeyVersionsAsync(NewKeyName, default))
      .Returns(ToAsyncPageable(keys.Select(k => k.Properties).TakeLast(2)));

    _keyClientMock
      .Setup(k => k.GetPropertiesOfKeysAsync(default))
      .Returns(ToAsyncPageable(keys.DistinctBy(k => k.Name).Select(k => k.Properties)));

    var activeKeys = await sut.GetActiveKeysAsync();

    activeKeys.Value.Select(ToKid).Should().BeEquivalentTo(new List<string>() {
      ToKid(keys[0]),
      ToKid(keys[2]),
      ToKid(keys[3]),
      ToKid(keys[6])
    });
  }

  private void Setup_GetKey(string key, string? version = null, KeyVaultKey? keyVaultKey = null) {
    _keyClientMock
      .Setup(client => client.GetKey(key, version, default))
      .Returns(Response.FromValue(keyVaultKey ?? KeyVaultKeyStub.Create(key, version ?? CurrentKeyVersion), null!));
  }

  private KeyVaultKeyProvider MakeSut() {
    return new KeyVaultKeyProvider(_keyClientMock.Object, _options);
  }

  private void ChangeOptionKeyName(string newName) {
    _options.ChangeOption(new KeyVaultSigingOptions() { SigningKeyName = newName });
  }

  private AsyncPageable<T> ToAsyncPageable<T>(IEnumerable<T> source) where T : notnull {
    return AsyncPageable<T>.FromPages(
      source.Select(s => Page<T>.FromValues(
        new List<T>() { s }.ToImmutableList(),
        default, null
      )));
  }

  private string ToKid(KeyVaultKey key) {
    return $"{key.Name}::{key.Properties.Version}";
  }
}