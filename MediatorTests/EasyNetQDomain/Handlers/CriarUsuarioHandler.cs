using EasyNetQ;
using EasyNetQDomain.Messages;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQDomain.Handlers
{
    public class CriarUsuarioHandler : 
        IRequestHandler<CriarUsuario, bool>,
        IRequestHandler<UsuarioCriado, bool>

    {
        private ILogger _logger;
        private IBus _bus;

        public CriarUsuarioHandler(ILogger logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task<bool> Handle(CriarUsuario request, CancellationToken cancellationToken)
        {
            await Task.Run(() => Console.WriteLine($"aki oh {request.Nome}"));
            var evt = new UsuarioCriado(DateTime.Now);
            _bus.Publish<UsuarioCriado>(evt);
            return true;
        }

        public async Task<bool> Handle(UsuarioCriado request, CancellationToken cancellationToken)
        {
            await Task.Run(() => Console.WriteLine($"criado em  {request.DtCriacao}"));
            return true;
        }
    }
}