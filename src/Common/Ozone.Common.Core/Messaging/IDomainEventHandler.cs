using MediatR;
using Ozone.Common.Domain.Messaging;

namespace Ozone.Common.Core.Messaging;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
  where TEvent : IDomainEvent { }