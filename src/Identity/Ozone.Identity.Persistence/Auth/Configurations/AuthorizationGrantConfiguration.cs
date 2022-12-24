using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Auth.Configurations;

public sealed class AuthorizationGrantConfiguration : IEntityTypeConfiguration<AuthorizationGrant> {
  public void Configure(EntityTypeBuilder<AuthorizationGrant> builder) {
    builder.ToTable(IdentityTableNames.AuthorizationGrant, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
  }
}