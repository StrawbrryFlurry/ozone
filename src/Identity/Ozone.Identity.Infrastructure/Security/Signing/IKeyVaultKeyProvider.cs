using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Infrastructure.Security.Signing;

public interface IKeyVaultKeyProvider {
  public string ActiveKid { get; }
  public string SigningKeyName { get; }

  public string FormatKeyVersion(KeyVaultKey key);

  /// <summary>
  /// Change the active CryptographyClient to
  /// use the most current key in Azure KeyVault 
  /// </summary>
  public Result RefreshActiveCryptographyClient();

  public Task<Result<List<KeyVaultKey>>> GetActiveKeysAsync(CancellationToken ct);

  public Result<CryptographyClient> GetClientForKid(string kid);
  public Result<SecurityTokenSigningMetadata> GetSigningMetadata(string kid);
}