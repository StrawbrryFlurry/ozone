using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Persistence.Auth.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken> {
  public void Configure(EntityTypeBuilder<RefreshToken> builder) {
    builder.ToTable(IdentityTableNames.RefreshToken, IdentityContext.DefaultSchema);
    builder.HasKey(r => r.Id);

    builder.Property(r => r.OpaqueToken).IsUnicode();
  }
}