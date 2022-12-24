namespace Ozone.Common.Domain.Data;

/// <summary>
/// Represents a persistence store
/// for <see cref="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> where TEntity : IAggregateRoot { }