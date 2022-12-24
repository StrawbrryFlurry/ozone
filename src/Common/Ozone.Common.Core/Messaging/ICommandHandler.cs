using MediatR;
using Ozone.Common.Functional;

namespace Ozone.Common.Core.Messaging;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, IResult>
  where TCommand : ICommand { }

public interface ICommandHandler<in TCommand, TResult>
  : IRequestHandler<TCommand, IResult<TResult>>
  where TCommand : ICommand<TResult> { }