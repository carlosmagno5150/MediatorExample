using Polly;
using Project.RabbitMQ.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;
using System;
using System.IO;
using System.Net.Sockets;

namespace Project.RabbitMQ
{
    public class DefaultRabbitMqPersistentConnection : IEventBusRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly int _retryCount;

        private IConnection _connection;

        private bool _disposed;

        private readonly object _syncRoot = new object();

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory,
                                                   ILogger logger,
                                                   int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        public bool IsConnectedToEventBus => (_connection != null && _connection.IsOpen) && !_disposed;

        public IModel CreateEventBusModel()
        {
            if (!IsConnectedToEventBus)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) { return; }

            if (disposing)
            {
                try
                {
                    _connection.Dispose();
                }
                catch (IOException ex)
                {
                    _logger.Fatal(ex.ToString());
                }
            }

            _disposed = true;
        }

        #region EventBus

        public bool TryConnectToEventBus()
        {
            _logger.Information("RabbitMQ Client is trying to connect to event bus");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.Warning(ex.ToString());
                    }
                );

                policy.Execute((Action)(() =>
                {
                    _connection = this._connectionFactory
                          .CreateConnection();
                }));

                if (IsConnectedToEventBus)
                {
                    _connection.ConnectionShutdown += OnEventBusConnectionShutdown;
                    _connection.CallbackException += OnEventBusCallbackException;
                    _connection.ConnectionBlocked += OnEventBusConnectionBlocked;

                    _logger.Information($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");

                    return true;
                }
                else
                {
                    _logger.Fatal("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        private void OnEventBusConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.Warning("A RabbitMQ event bus connection is shutdown. Trying to re-connect...");

            TryConnectToEventBus();
        }

        private void OnEventBusCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.Warning("A RabbitMQ event bus connection throw exception. Trying to re-connect...");

            TryConnectToEventBus();
        }

        private void OnEventBusConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.Warning("A RabbitMQ event bus connection is on shutdown. Trying to re-connect...");

            TryConnectToEventBus();
        }

        #endregion EventBus

        //#region CommandBus

        //public bool TryConnectToCommandBus()
        //{
        //    _logger.Information("RabbitMQ Client is trying to connect to command bus queue");

        //    lock (_syncRoot)
        //    {
        //        var policy = Policy.Handle<SocketException>()
        //            .Or<BrokerUnreachableException>()
        //            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
        //            {
        //                _logger.Warning(ex.ToString());
        //            }
        //        );

        //        policy.Execute((Action)(() =>
        //        {
        //            _commandBusConnection = this._connectionFactory
        //                  .CreateConnection();
        //        }));

        //        if (IsConnectedToCommandBus)
        //        {
        //            _commandBusConnection.ConnectionShutdown += OnCommandBusConnectionShutdown;
        //            _commandBusConnection.CallbackException += OnCommandBusCallbackException;
        //            _commandBusConnection.ConnectionBlocked += OnCommandBusConnectionBlocked;

        //            _logger.Information($"RabbitMQ persistent connection acquired a connection {_commandBusConnection.Endpoint.HostName} and is subscribed to failure commands");

        //            return true;
        //        }
        //        else
        //        {
        //            _logger.Fatal("FATAL ERROR: RabbitMQ connections could not be created and opened");

        //            return false;
        //        }
        //    }
        //}

        //private void OnCommandBusConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        //{
        //    if (_disposed) return;

        //    _logger.Warning("A RabbitMQ command bus connection is shutdown. Trying to re-connect...");

        //    TryConnectToCommandBus();
        //}

        //void OnCommandBusCallbackException(object sender, CallbackExceptionEventArgs e)
        //{
        //    if (_disposed) return;

        //    _logger.Warning("A RabbitMQ  command bus connection threw exception. Trying to re-connect...");

        //    TryConnectToCommandBus();
        //}

        //void OnCommandBusConnectionShutdown(object sender, ShutdownEventArgs reason)
        //{
        //    if (_disposed) return;

        //    _logger.Warning("A RabbitMQ  command bus connection is on shutdown. Trying to re-connect...");

        //    TryConnectToCommandBus();
        //}

        //#endregion
    }
}