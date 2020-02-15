using MediatR;

namespace Project.Cqrs.Events
{
    public interface IEventHandler<in T> : INotificationHandler<T> where T : IEvent
    {

    }
}