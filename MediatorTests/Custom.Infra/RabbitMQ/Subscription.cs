using System;
using System.Collections.Generic;
using System.Text;

namespace Custom.Infra.RabbitMQ
{
    public class Subscription
    {
        private readonly IEventBusRabbitMqPersistentConnection _rabbit;

        public void Subscribe()
        {
            var channel = _rabbit.CreateEventBusModel();

            channel.ExchangeDeclare(exchange: _brokerName,
                                    type: "x-delayed-message",
                                    durable: true,
                                    autoDelete: false,
                                    arguments: new Dictionary<string, object> { { "x-delayed-type", "direct" } });

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var commandName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessCommand(commandName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };
        }
    }
}
