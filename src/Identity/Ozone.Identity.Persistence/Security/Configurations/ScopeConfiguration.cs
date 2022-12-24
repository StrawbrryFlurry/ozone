using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Security.Configurations;

public sealed class ScopeConfiguration : IEntityTypeConfiguration<Scope> {
  public void Configure(EntityTypeBuilder<Scope> builder) {
    builder.ToTable(IdentityTableNames.Scope, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
    builder.OwnsOne(x => x.ScopeDescriptor);
  }
}