using System.Collections.Concurrent;
using System.Security.Cryptography;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Infrastructure.Security.Signing;

public sealed class KeyVaultKeyProvider : IKeyVaultKeyProvider {
  private readonly KeyClient _client;
  private readonly IOptionsMonitor<KeyVaultSigingOptions> _options;

  private ConcurrentDictionary<string, KeyVaultKey> _keyCache = new();

  /// <summary>
  /// A dictionary containing all clients that were
  /// used to sign a given token. If we encounter tokens
  /// that were signed with a different token than the
  /// one that is currently used, it is also stored within
  /// this dictionary. The current client is determined by
  /// <see cref="_activeKid"/> which is also referenced
  /// as the KeyId in the token.
  /// </summary>
  private readonly ConcurrentDictionary<string, CryptographyClient> _cryptoClients = new();

  private readonly object _refreshCryptoClientLock = new();
  private string _activeKid;

  public string ActiveKid => _activeKid;
  public string SigningKeyName => _options.CurrentValue.SigningKeyName;

  protected KeyVaultKeyProvider(KeyClient client) {
    _client = client;
  }

  public KeyVaultKeyProvider(
    KeyClient client,
    IOptionsMonitor<KeyVaultSigingOptions> options
  ) {
    _client = client;
    _options = options;

    ArgumentException.ThrowIfNullOrEmpty(SigningKeyName, nameof(SigningKeyName));

    _activeKid = FormatKeyVersion(GetCurrentKeyVaultKey());
  }

  private KeyVaultKey GetCurrentKeyVaultKey() {
    var keyResult = _client.GetKey(SigningKeyName).Value;
    _keyCache.TryAdd(FormatKeyVersion(keyResult), keyResult);
    return keyResult;
  }

  /// <summary>
  /// Change the active CryptographyClient to
  /// use the most current key in Azure KeyVault 
  /// </summary>
  public Result RefreshActiveCryptographyClient() {
    lock (_refreshCryptoClientLock) {
      var activeKey = GetCurrentKeyVaultKey();
      var kid = FormatKeyVersion(activeKey);
      var cryptoClientResult = GetClientForKid(kid);

      if (cryptoClientResult.IsFailure) {
        return cryptoClientResult.Error;
      }

      _activeKid = kid;
      _cryptoClients.TryAdd(_activeKid, cryptoClientResult.Value);
    }

    return Result.Success();
  }

  public async Task<Result<List<KeyVaultKey>>> GetActiveKeysAsync(CancellationToken ct = default) {
    var keyProperties = _client.GetPropertiesOfKeysAsync(ct);
    var keys = new List<KeyVaultKey>();

    await foreach (var key in keyProperties) {
      if (key.Enabled == false) {
        continue;
      }

      await GetAllActiveKeysOfName(key.Name, keys, ct);
    }

    return keys;
  }

  private async Task<Result> GetAllActiveKeysOfName(string name, List<KeyVaultKey> accumulator, CancellationToken ct) {
    var versions = _client.GetPropertiesOfKeyVersionsAsync(name, ct);

    await foreach (var key in versions) {
      if (!key.Enabled == true) {
        continue;
      }

      if (key.ExpiresOn.GetValueOrDefault() < DateTimeOffset.Now) {
        continue;
      }


      var keyVaultKey = GetKeyVaultKeyByKid($"{key.Name}::{key.Version}");
      accumulator.Add(keyVaultKey);
    }

    return Result.Success();
  }

  public string FormatKeyVersion(KeyVaultKey key) {
    return $"{key.Name}::{key.Properties.Version}";
  }

  public Result<CryptographyClient> GetClientForKid(string kid) {
    try {
      return _cryptoClients.GetOrAdd(kid, (k) => MakeCryptographyClientForKey(GetKeyDataFromKid(k)));
    }
    catch (Exception ex) {
      return new Error("Identity.Sign", $"Key with id {kid} was not found.", ex);
    }
  }

  private CryptographyClient MakeCryptographyClientForKey((string keyName, string version) data) {
    return _client.GetCryptographyClient(data.keyName, data.version);
  }

  public Result<SecurityTokenSigningMetadata> GetSigningMetadata(string kid) {
    KeyVaultKey key;
    try {
      key = _keyCache.GetOrAdd(kid, GetKeyVaultKeyByKid(kid));
    }
    catch (Exception ex) {
      return new Error("Identity.Sign", $"Key with id {kid} was not found.", ex);
    }

    var algorithm = GetAlgorithmForKey(key);

    if (algorithm is null) {
      return new Error("Identity.Sign", $"Unknown key algorithm {key.KeyType}");
    }

    return new SecurityTokenSigningMetadata() {
      Algorithm = SigningAlgorithm.Parse(algorithm).Value,
      KeyId = kid
    };
  }

  private AsymmetricAlgorithm? GetAlgorithmForKey(KeyVaultKey key) {
    var type = key.KeyType;

    return type switch {
      _ when type == KeyType.Ec => key.Key.ToECDsa(),
      _ when type == KeyType.Rsa => key.Key.ToRSA(),
      _ => null
    };
  }

  internal KeyVaultKey GetKeyVaultKeyByKid(string kid) {
    return _keyCache.GetOrAdd(kid, (_) => GetKeyVaultKeyByKeyData(GetKeyDataFromKid(kid)));
  }

  private (string keyName, string version) GetKeyDataFromKid(string kid) {
    var keySegments = kid.Split("::");
    return (keySegments[0], keySegments[1]);
  }

  private KeyVaultKey GetKeyVaultKeyByKeyData((string keyName, string version) data) {
    return _client.GetKey(data.keyName, data.version).Value;
  }
}