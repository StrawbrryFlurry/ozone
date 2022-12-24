using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Security.Configurations;

public sealed class ServiceActionConfiguration : IEntityTypeConfiguration<ServiceAction> {
  public void Configure(EntityTypeBuilder<ServiceAction> builder) {
    builder.ToTable(IdentityTableNames.ServiceAction, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
    builder.OwnsOne(x => x.ActionDescriptor);
  }
}