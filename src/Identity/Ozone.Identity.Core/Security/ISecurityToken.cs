namespace Ozone.Identity.Core.Security;

public interface ISecurityToken {
  public string ClientId { get; }
  public string Issuer { get; }
  public DateTimeOffset IssuedAt { get; }
  public DateTimeOffset ValidFrom { get; }
  public DateTimeOffset ValidTo { get; }
}