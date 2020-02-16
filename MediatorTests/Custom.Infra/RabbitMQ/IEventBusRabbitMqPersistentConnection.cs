using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Custom.Infra.RabbitMQ
{
    public interface IEventBusRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnectedToEventBus { get; }

        bool TryConnectToEventBus();

        IModel CreateEventBusModel();
    }
}
