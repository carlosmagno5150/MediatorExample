using MediatR;

namespace Project.Cqrs.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
