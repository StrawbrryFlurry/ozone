using MediatR;
using Ozone.Common.Functional;

namespace Ozone.Common.Core.Messaging;

public interface IQuery<TResult> : IRequest<IResult<TResult>> { }