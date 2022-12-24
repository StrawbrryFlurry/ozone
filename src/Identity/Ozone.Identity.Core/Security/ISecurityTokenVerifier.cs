using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Security;

public interface ISecurityTokenVerifier {
  public Result Verify(string token);
  public Result VerifyAndDecodeToken(string token);
}