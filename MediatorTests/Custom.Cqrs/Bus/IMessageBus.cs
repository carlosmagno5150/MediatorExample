using Custom.Cqrs.Commands;
using Custom.Cqrs.Events;
using System;
using System.Threading.Tasks;

namespace Custom.Cqrs.Bus
{
    public interface IMessageBus : IDisposable
    {
        Task SendCommand<T>(T command) where T : ICommand;

        Task RaiseEvent<T>(T @event) where T : IEvent;
    }
}