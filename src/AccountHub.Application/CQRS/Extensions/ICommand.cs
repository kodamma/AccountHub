using MediatR;

namespace AccountHub.Application.CQRS.Extensions
{
    public interface ICommand : IRequest;
    public interface ICommand<T> : IRequest<T>;
}
