using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Persistence.Auth.Configurations;

public sealed class AuthorizationCodeConfiguration : IEntityTypeConfiguration<AuthorizationCode> {
  public void Configure(EntityTypeBuilder<AuthorizationCode> builder) {
    builder.ToTable(IdentityTableNames.AuthorizationCode, IdentityContext.DefaultSchema);
    builder.HasKey(c => c.Id);

    builder.OwnsOne(c => c.CodeChallengeMode);
  }
}