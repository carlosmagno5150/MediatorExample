using Custom.Cqrs.Bus;
using Custom.Cqrs.Commands;
using Custom.Cqrs.Events;
using MediatR;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Custom.Infra.Bus
{
    public class MessageBus : IMessageBus
    {

        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        public MessageBus(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task RaiseEvent<T>(T @event) where T : IEvent
        {
            _logger.Information($"raising event {@event.GetType().Name}");
            await _mediator.Publish(@event);
        }

        public async Task SendCommand<T>(T command) where T : ICommand
        {
            await _mediator.Send(command);
        }
    }
}