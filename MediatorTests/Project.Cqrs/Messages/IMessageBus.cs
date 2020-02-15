using System;
using System.Threading.Tasks;
using Project.Cqrs.Commands;
using Project.Cqrs.Events;

namespace Project.Cqrs.Messages
{
    public interface IMessageBus : IDisposable
    {
        Task SendCommand<T>(T command) where T : ICommand;

        Task RaiseEvent<T>(T @event) where T : IEvent;
    }
}