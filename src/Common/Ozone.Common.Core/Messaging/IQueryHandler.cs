using MediatR;
using Ozone.Common.Functional;

namespace Ozone.Common.Core.Messaging;

public interface IQueryHandler<TQuery, TResponse>
  : IRequestHandler<TQuery, IResult<TResponse>>
  where TQuery : IQuery<TResponse> { }