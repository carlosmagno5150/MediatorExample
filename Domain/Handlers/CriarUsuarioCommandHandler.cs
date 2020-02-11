using System.Threading;
using System.Threading.Tasks;
using Domain.Commands;
using Domain.Model;
using MediatR;
using Serilog;

namespace Domain.Handlers
{
    public class CriarUsuarioCommandHandler : IRequestHandler<CriarUsuarioCommand, ResponseResult>
    {
        private ILogger _logger;

        public CriarUsuarioCommandHandler(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<ResponseResult> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Handling command");
            return new ResponseResult() { Success =  true};
        }
    }
}