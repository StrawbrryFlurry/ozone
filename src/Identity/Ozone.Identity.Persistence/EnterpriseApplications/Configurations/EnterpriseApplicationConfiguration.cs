using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.EnterpriseApplications;

namespace Ozone.Identity.Persistence.EnterpriseApplications.Configurations;

public sealed class EnterpriseApplicationConfiguration : IEntityTypeConfiguration<EnterpriseApplication> {
  public void Configure(EntityTypeBuilder<EnterpriseApplication> builder) {
    builder.ToTable(IdentityTableNames.EnterpriseApplication, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
  }
}