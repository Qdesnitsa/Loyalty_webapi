using MediatR;

namespace Loyalty.Application;

public interface ICommand<out TResult> : IRequest<TResult>;

public interface IQuery<out TResult> : IRequest<TResult>;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
