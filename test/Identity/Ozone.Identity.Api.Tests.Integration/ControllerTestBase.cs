using Microsoft.Extensions.DependencyInjection;
using Ozone.Common.Testing.Api;

namespace Ozone.Identity.Api.Tests.Integration;

public abstract class ControllerTestBase : ControllerTestBase<AssemblyReference> {
  public override void ConfigureServices(IServiceCollection services) {
    services.AddApi();
    base.ConfigureServices(services);
  }
}