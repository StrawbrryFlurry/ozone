using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Infrastructure.Security.Signing;

public sealed class KeyVaultTokenSigner : ISecurityTokenSigner {
  private static readonly SigningAlgorithm Algorithm = SigningAlgorithm.Default;

  private readonly IKeyVaultKeyProvider _provider;

  public KeyVaultTokenSigner(
    IKeyVaultKeyProvider provider
  ) {
    _provider = provider;
  }

  public bool SupportsJwk => true;

  /// <summary>
  /// Creates a signature of the <see cref="ISignableSecurityToken"/>
  /// returning the Base64 signature of it's data.
  /// </summary>
  /// <param name="token"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  public async Task<Result<ISecurityTokenSignResult>> SignAsBase64Async(
    ISignableSecurityToken token,
    CancellationToken ct = default
  ) {
    var signResult = await SingInternalAsync(token, ct);

    if (signResult.IsFailure) {
      return signResult.Error;
    }

    var signature = signResult.Value.Signature;
    return new SecurityTokenSignResult(signature.ToBase64UrlEncoded());
  }

  private async Task<Result<SignResult>> SingInternalAsync(
    ISignableSecurityToken token,
    CancellationToken ct = default,
    int retryCount = 0
  ) {
    if (retryCount > 3) {
      return new Error("Identity.Sign", "Could not find a valid signing key");
    }

    var signature = await SignWithActiveClientAsync(token, ct);

    if (signature.IsSuccess) {
      return signature.Value;
    }

    _provider.RefreshActiveCryptographyClient();
    retryCount++;
    return await SingInternalAsync(token, ct, retryCount).ConfigureAwait(false);
  }

  private async Task<Result<SignResult>> SignWithActiveClientAsync(
    ISignableSecurityToken token,
    CancellationToken ct = default
  ) {
    // We deliberately capture the current signing key here because it might
    // be changed while hashing the token
    var keyId = _provider.ActiveKid;
    var signingMetadata = _provider.GetSigningMetadata(keyId);

    if (signingMetadata.IsFailure) {
      return signingMetadata.Error;
    }

    var dataToSign = token.ToSignableHash(signingMetadata.Value);

    if (dataToSign.IsFailure) {
      return dataToSign.Error;
    }

    var client = _provider.GetClientForKid(keyId).Value;

    try {
      return await client.SignAsync(Algorithm.ToString(), dataToSign.Value, ct);
    }
    catch (Exception ex) {
      return new Error("Identity.Sign", "An error occured while signing the token", ex);
    }
  }

  public async Task<Result<List<ISecurityTokenSigningKey>>> GetSingingKeysAsync(CancellationToken ct = default) {
    var keys = await _provider.GetActiveKeysAsync(ct);
    var signingKeys = new List<ISecurityTokenSigningKey>();

    foreach (var key in keys.Value) {
      var keyInfo = CreateSigningKeyInfo(key);

      if (keyInfo.IsFailure) {
        return keyInfo.Error;
      }

      signingKeys.Add(keyInfo.Value);
    }

    return signingKeys;
  }

  private Result<ISecurityTokenSigningKey> CreateSigningKeyInfo(KeyVaultKey key) {
    var keyId = _provider.FormatKeyVersion(key);
    var type = key.KeyType;

    return type switch {
      _ when type == KeyType.Rsa => new RsaSecurityTokenSingingKey(keyId, key.Key.ToRSA()),
      _ when type == KeyType.Ec => new EcSecurityTokenSigningKey(keyId, key.Key.ToECDsa()),
      _ => new Error("Identity.Sign", "Cannot serialize unknown key type")
    };
  }

  public async Task<Result> VerifyAsync(ISignableSecurityToken token) {
    var client = _provider.GetClientForKid(token.SigningKey!);

    if (client.IsFailure) {
      return client.Error;
    }

    var hash = token.ToSignableHash();

    if (hash.IsFailure) {
      return hash.Error;
    }

    var signature = Encoding.Unicode.GetBytes(token.Signature!);

    var result = await client.Value.VerifyAsync(token.SigningAlgorithm.ToString(), hash.Value, signature,
      CancellationToken.None);

    return result.IsValid
      ? Result.Success()
      : new Error("Identity.Sign", "The signature of the jwt provided is invalid.");
  }
}