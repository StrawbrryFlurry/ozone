namespace Ozone.Common.Domain.Data;

/// <summary>
/// Represents the root entity of an aggregate.
/// Aggregates are a collection of entities that are treated as a unit.
/// If an aggregate is deleted, all entities in the aggregate are deleted.
/// If the domain entity does not consist of multiple elements / entities, it is itself
/// the root entity and thus, should implement this interface.
/// https://martinfowler.com/bliki/DDD_Aggregate.html
/// </summary>
public interface IAggregateRoot { }