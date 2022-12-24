namespace Ozone.Identity.Core.Security.Signing;

public interface ISecurityTokenSigningKey {
  public string KeyType { get; }
  public SigningAlgorithm Algorithm { get; }
  public string KeyId { get; }
  public string Usage { get; }

  public string ToJsonJwkEntry();
}