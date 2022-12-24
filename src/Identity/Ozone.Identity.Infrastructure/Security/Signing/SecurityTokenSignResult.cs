using Ozone.Identity.Core.Security;

namespace Ozone.Identity.Infrastructure.Security.Signing;

public sealed record SecurityTokenSignResult(string Signature) : ISecurityTokenSignResult { }