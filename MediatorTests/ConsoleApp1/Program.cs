using System;
using System.Threading.Tasks;
using Domain.Commands;
using Domain.Handlers;
using Domain.PipelineBehavior;
using Domain.Validation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace ConsoleApp1
{
    public class Container
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                .AddMediatR(typeof(Program).Assembly, typeof(CriarUsuario).Assembly)
                .AddScoped<Initiator>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddValidatorsFromAssembly(typeof(CriarUsuarioValidation).Assembly)
                .AddScoped<>
                .AddSingleton<ILogger>(s=> new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("C:\\Log\\log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger())
                .BuildServiceProvider();
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var prov = Container.Configure();
            var ini = (Initiator)prov.GetService(typeof(Initiator));
            ini.Go();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }

    public class Initiator
    {
        private IMediator _mediator;
        private ILogger _logger;

        public Initiator(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public void Go()
        {
            var cmd = new CriarUsuarioCommand(1, "Usuario");
            _logger.Information("sending to mediator");
            var result = Task.Run(()=>  _mediator.Send(cmd));
            result.Wait();
            _logger.Information($"result: {result.Result.Success}");
        }
    }
}
