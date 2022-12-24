using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.EnterpriseApplications;

namespace Ozone.Identity.Persistence.EnterpriseApplications.Configurations;

public sealed class ServiceApplicationConfiguration : IEntityTypeConfiguration<ServiceApplication> {
  public void Configure(EntityTypeBuilder<ServiceApplication> builder) {
    builder.ToTable(IdentityTableNames.ServiceApplication, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
  }
}