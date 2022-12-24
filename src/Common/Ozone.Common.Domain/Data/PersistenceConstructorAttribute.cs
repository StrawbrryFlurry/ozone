namespace Ozone.Common.Domain.Data;

/// <summary>
/// Marks the constructor as the constructor used
/// for the persistence framework.
/// Note: This constructor is not guaranteed to
/// fully initialize the target object and is meant
/// to re-create already existing entities.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class PersistenceConstructorAttribute : Attribute { }