using MediatR;

namespace AccountHub.Application.CQRS.Extensions
{
    public interface ICommandHandler<T> : IRequestHandler<T>
        where T : ICommand;

    public interface ICommandHandler<T, R> : IRequestHandler<T, R>
        where T : ICommand<R>;
}
