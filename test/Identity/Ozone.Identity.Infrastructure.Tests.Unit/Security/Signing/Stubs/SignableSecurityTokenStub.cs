using System.Text;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Security;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Infrastructure.Tests.Unit.Security.Signing;

public sealed class SignableSecurityTokenStub : ISignableSecurityToken {
  private readonly string _tokenData;
  public string ClientId { get; }
  public string Issuer { get; }
  public DateTimeOffset IssuedAt { get; }
  public DateTimeOffset ValidFrom { get; }
  public DateTimeOffset ValidTo { get; }
  public string? Signature { get; }
  public SigningAlgorithm? SigningAlgorithm { get; }

  public SignableSecurityTokenStub(string tokenData) {
    _tokenData = tokenData;
  }

  public string KeyId { get; set; }

  public string? SigningKey { get; }

  Task<Result> ISignableSecurityToken.SignAsync(ISecurityTokenSigner signer, CancellationToken ct) {
    throw new NotImplementedException();
  }

  public Result<byte[]> ToSignableHash() {
    return Encoding.Default.GetBytes(_tokenData);
  }

  public Result<byte[]> ToSignableHash(SecurityTokenSigningMetadata metadata) {
    return Encoding.Default.GetBytes(_tokenData);
  }
}