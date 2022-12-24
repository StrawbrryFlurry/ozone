using Ozone.Common.Functional;

namespace Ozone.Common.Domain.Identity;

public static class IdentityDescriptorErrors {
  public static Error InvalidIdentityDescriptorFormat = new(
    "InvalidIdentityDescriptorFormat",
    "The identity descriptor format is invalid."
  );
}