using System;
using RabbitMQ.Client;

namespace Project.RabbitMQ.Interface
{
    public interface IEventBusRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnectedToEventBus { get; }

        bool TryConnectToEventBus();

        IModel CreateEventBusModel();
    }
}
