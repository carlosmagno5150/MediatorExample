using Domain.Commands;
using MediatR;
using Project.Cqrs.Messages;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Domain.Events;
using Project.Cqrs.DomainNotification;

// ReSharper disable IdentifierTypo

namespace Domain.Handlers
{
    public class CriarUsuarioHandler : IRequestHandler<CriarUsuario>
    {
        private readonly ILogger _logger;
        public readonly IMessageBus _messageBus;

        public CriarUsuarioHandler(ILogger logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        public async Task<Unit> Handle(CriarUsuario request, CancellationToken cancellationToken)
        {
            _logger.Information("Handling command");
            await _messageBus.RaiseEvent(new UsuarioCriado("Usuario", "Criado"));
            await _messageBus.RaiseEvent(new DomainNotification("myKey", "chum"));
            return await Unit.Task;
        }
    }
}