using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Identity;
using Ozone.Identity.Domain.Identity.ValueObjects;

namespace Ozone.Identity.Persistence.Identity.Configurations;

public sealed class OzoneUserConfiguration : IEntityTypeConfiguration<OzoneUser> {
  public void Configure(EntityTypeBuilder<OzoneUser> builder) {
    builder.ToTable(IdentityTableNames.OzoneUser, IdentityContext.DefaultSchema);

    builder.Property(x => x.Id).HasConversion(x => x.ToString(), x => UserIdentifier.CreateFrom(x).Value);
    builder.HasKey(x => x.Id);
  }
}