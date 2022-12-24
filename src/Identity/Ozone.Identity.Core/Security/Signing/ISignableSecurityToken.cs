using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Security.Signing;

public record SecurityTokenSigningMetadata {
  public required string KeyId { get; init; }
  public required SigningAlgorithm Algorithm { get; init; }
}

public interface ISignableSecurityToken : ISecurityToken {
  public string? SigningKey { get; }
  public string? Signature { get; }
  public SigningAlgorithm? SigningAlgorithm { get; }
  public Task<Result> SignAsync(ISecurityTokenSigner signer, CancellationToken ct = default);
  public Result<byte[]> ToSignableHash();
  public Result<byte[]> ToSignableHash(SecurityTokenSigningMetadata metadata);
}