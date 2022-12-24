using System.Reflection;
using Ozone.Common.Domain.Data;

namespace Ozone.Common.Testing.Domain.Entity;

public static class EntityMockFactory {
  public static TEntity CreateInstance<TEntity>() {
    var entityCtor = typeof(TEntity)
      .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
      .SingleOrDefault(c => c.GetCustomAttribute<PersistenceConstructorAttribute>() is not null);

    if (entityCtor is null) {
      throw new InvalidOperationException(
        $"Entity type {typeof(TEntity).Name} is missing persistence constructor with {nameof(PersistenceConstructorAttribute)}");
    }

    return (TEntity)entityCtor.Invoke(Array.Empty<object>());
  }
}