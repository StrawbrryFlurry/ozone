using Ozone.Common.Functional;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Security;

public interface ISecurityTokenSigner {
  public bool SupportsJwk { get; }

  public Task<Result<ISecurityTokenSignResult>> SignAsBase64Async(ISignableSecurityToken token,
    CancellationToken ct = default);

  public Task<Result<List<ISecurityTokenSigningKey>>> GetSingingKeysAsync(CancellationToken ct = default);
  public Task<Result> VerifyAsync(ISignableSecurityToken token);
}