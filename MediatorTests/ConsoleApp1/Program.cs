using System;
using System.Threading.Tasks;
using Domain.Commands;
using Domain.Handlers;
using Domain.PipelineBehavior;
using Domain.Validation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Project.Cqrs.DomainNotification;
using Project.Cqrs.Messages;
using Serilog;
using Serilog.Core;

namespace ConsoleApp1
{
    public class Container
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                .AddMediatR(typeof(Program).Assembly, 
                typeof(CriarUsuario).Assembly,
                typeof(DomainNotificationHandler).Assembly)
                .AddScoped<IMessageBus, MessageBus>()// IEventBus
                .AddScoped<Initiator>()
                .AddSingleton<DomainNotificationHandler>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddValidatorsFromAssembly(typeof(CriarUsuarioValidation).Assembly)
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
        private DomainNotificationHandler _domainNotification;

        public Initiator(IMediator mediator, ILogger logger, DomainNotificationHandler dn)
        {
            _mediator = mediator;
            _logger = logger;
            _domainNotification = dn;
        }

        public void Go()
        {
            var cmd = new CriarUsuario(1, "Usuario");
            _logger.Information("sending to mediator");
            var has = _domainNotification.HasNotifications();
            var result = Task.Run(()=>  _mediator.Send(cmd));
            result.Wait();
            _logger.Information($"Usuario criado");
             has = _domainNotification.HasNotifications();

        }
    }
}
