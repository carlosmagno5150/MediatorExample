using EasyNetQ;
using EasyNetQDomain.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace EasyNetQConsole
{
    public class DI
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                .AddMediatR(typeof(Program).Assembly, typeof(CriarUsuario).Assembly)
                .AddScoped<Initiator>()
                .AddSingleton<ILogger>(s => new LoggerConfiguration()
                    .WriteTo.Console()
                    //.WriteTo.File("C:\\Log\\log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger())
                .RegisterEasyNetQ("host=localhost")
                .BuildServiceProvider();
        }
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            var prov = DI.Configure();
            var ini = (Initiator)prov.GetService(typeof(Initiator));
            ini.Go();
        }
    }

    public class Initiator
    {
        private IMediator _mediator;
        private ILogger _logger;
        private IBus _bus;

        public Initiator(IMediator mediator, ILogger logger, IBus bus)
        {
            _mediator = mediator;
            _logger = logger;
            _bus = bus;
        }

        public void Go()
        {
            //using (var bus = RabbitHutch.CreateBus("host=localhost"))
            //{
            //bus.Subscribe<CriarUsuario>("MyGame", msg =>
            //{
            //    new CriarUsuarioHandler().Handler(msg);
            //    Console.WriteLine("msg");
            //});
            _bus.SubscribeAsync<UsuarioCriado>("this", async (msg) =>
            {
                await _mediator.Send(msg);
            });
            _bus.SubscribeAsync<CriarUsuario>("this", async (msg) =>
            {
                await _mediator.Send(msg);
            });

            //for (int i = 0; i < 5000; i++)
            //{
            //    _bus.Publish(new CriarUsuario() { Nome = $"{i} pessoa" });
            //    //var t = Task.Delay(1000);
            //    //t.Wait();
            //}
            Console.WriteLine("fim");
            Console.ReadLine();
           

            //}
        }
    }
}