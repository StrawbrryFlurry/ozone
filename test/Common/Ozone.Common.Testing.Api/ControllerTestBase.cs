using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Ozone.Common.Core.Messaging;
using Ozone.Common.Functional;

namespace Ozone.Common.Testing.Api;

public abstract class ControllerTestBase<TApplicationStartup> where TApplicationStartup : class {
  public ControllerTestBase() {
    var factory = new WebApplicationFactory<TApplicationStartup>()
      .WithWebHostBuilder(builder => {
        ConfigureBuilder(builder);
        builder.ConfigureServices(ConfigureServicesCore);
      });

    Client = factory.CreateClient();
  }

  protected Mock<IMediator> HandlerMock { get; } = new();
  protected HttpClient Client { get; }

  public virtual void ConfigureServices(IServiceCollection services) { }

  private void ConfigureServicesCore(IServiceCollection services) {
    services.RemoveAll(typeof(IMediator));
    services.AddScoped(_ => HandlerMock.Object);
    ConfigureServices(services);
  }

  public virtual void ConfigureBuilder(IWebHostBuilder builder) { }

  public void SetupHandlerForCommand<TCommand>(Error? error = null) where TCommand : ICommand {
    HandlerMock
      .Setup(x => x.Send(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(error.HasValue ? Result.Failure(error.Value) : Result.Success());
  }

  public void SetupHandlerForCommand<TCommand, TResult>(TResult result) where TCommand : ICommand<TResult> {
    HandlerMock
      .Setup(x => x.Send(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Success(result));
  }

  public void SetupHandlerForCommand<TCommand, TResult>(Error error) where TCommand : ICommand<TResult> {
    HandlerMock
      .Setup(x => x.Send(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Failure<TResult>(error));
  }


  public void SetupHandlerForQuery<TQuery, TResult>(TResult result) where TQuery : IQuery<TResult> {
    HandlerMock
      .Setup(x => x.Send(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Success(result));
  }

  public void SetupHandlerForQuery<TQuery, TResult>(Error error) where TQuery : IQuery<TResult> {
    HandlerMock
      .Setup(x => x.Send(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Result.Failure<TResult>(error));
  }
}