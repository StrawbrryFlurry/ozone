using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Ozone.Common.Testing.Persistence;

public abstract class DbContextFactory<T> where T : DbContext {
  private static T? _instance;
  public static T Instance => Create();

  private static T GetOrCreate() {
    return _instance ??= Create();
  }

  public static T Create(Action<DbContextOptionsBuilder>? builderAction = null) {
    var optionsBuilder = new DbContextOptionsBuilder<T>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString());

    if (builderAction is not null) {
      builderAction(optionsBuilder);
    }

    var context = (T)Activator.CreateInstance(typeof(T), new object?[] { optionsBuilder.Options })!;
    context.Database.EnsureCreated();

    return context;
  }

  public static DbSet<TSet> DbSet<TSet>() where TSet : class {
    return Instance.Set<TSet>();
  }
}