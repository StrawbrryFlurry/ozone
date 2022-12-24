using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Token.Commands;

public record TokenGrantCommand : ICommand<TokenGrantCommandResult> {
  public string GrantType { get; set; }
}