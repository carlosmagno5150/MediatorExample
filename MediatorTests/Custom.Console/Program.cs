using Custom.Cqrs.Bus;
using Custom.Domain.Commands;
using Custom.Infra.Bus;
using Custom.Infra.RabbitMQ;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using System.Threading.Tasks;

namespace Custom.Console
{
    public class Container
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                .AddMediatR(typeof(Program).Assembly, typeof(CriarCliente).Assembly)
                .AddScoped<IMessageBus, MessageBus>()
                .AddScoped<Initiator>()
                .AddSingleton<IConnectionFactory>(svc => new ConnectionFactory() {
                    HostName = "Localhost" })
                .AddSingleton<IEventBusRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>()
                .AddSingleton<ILogger>(s => new LoggerConfiguration()
                    .WriteTo.Console()
                    //.WriteTo.File("C:\\Log\\log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger())
                .BuildServiceProvider();
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var prov = Container.Configure();
            var ini = (Initiator)prov.GetService(typeof(Initiator));
            Task.Run(() => ini.Go()).Wait();
        }
    }

    public class Initiator
    {
        private IMediator _mediator;
        private ILogger _logger;
        private readonly IMessageBus _bus;

        public Initiator(IMediator mediator, ILogger logger, IMessageBus bus)
        {
            _mediator = mediator;
            _logger = logger;
            _bus = bus;
        }

        public async Task Go()
        {
            _logger.Information("Inicio");
            await _bus.SendCommand(
                new CriarCliente("Novo Usuario", "12312312312", "email@email.com")
            );
        }
    }
}