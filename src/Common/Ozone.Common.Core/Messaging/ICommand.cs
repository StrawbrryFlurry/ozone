using MediatR;
using Ozone.Common.Functional;

namespace Ozone.Common.Core.Messaging;

public interface ICommand : IRequest<IResult> { }

public interface ICommand<TResponse> : IRequest<IResult<TResponse>> { }