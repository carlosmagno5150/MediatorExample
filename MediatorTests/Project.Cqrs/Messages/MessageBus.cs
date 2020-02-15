using MediatR;
using Project.Cqrs.Abstractions;
using Project.Cqrs.Commands;
using Project.Cqrs.Events;
using System;
using System.Threading.Tasks;

namespace Project.Cqrs.Messages
{
    public class MessageBus : IMessageBus
    {
        private readonly IEventBus _eventBus;
        private readonly IEventLogService _eventLogService;
        private readonly IMediator _mediator;
        private bool _disposed;

        public MessageBus(IMediator mediator, IEventBus eventBus = null, IEventLogService eventLogService = null)
        {
            _mediator = mediator;
            _eventBus = eventBus;
            _eventLogService = eventLogService;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task RaiseEvent<T>(T @event) where T : IEvent
        {
            if (_eventBus != null)
                _eventBus.Publish(@event);
            else
                _mediator.Publish(@event);

            return _eventLogService != null ? _eventLogService.MarkEventAsPublishedAsync(@event) : Task.CompletedTask;
        }

        public Task SendCommand<T>(T command) where T : ICommand
        {
            return _mediator.Send(command);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
    }
}