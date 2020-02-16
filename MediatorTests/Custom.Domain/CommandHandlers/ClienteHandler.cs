using Custom.Cqrs.Bus;
using Custom.Domain.Commands;
using Custom.Domain.Events;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Custom.Domain.CommandHandlers
{
    public class ClienteHandler : IRequestHandler<CriarCliente>
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _bus;

        public ClienteHandler(ILogger logger, IMessageBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task<Unit> Handle(CriarCliente request, CancellationToken cancellationToken)
        {
            _logger.Information($"handling request {request.Nome}");
            await _bus.RaiseEvent(new ClienteCriado(1, "novo cliente"));
            return await Unit.Task;
        }
    }
}