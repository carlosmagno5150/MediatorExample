using EasyNetQ;
using EasyNetQ.ConnectionString;
using EasyNetQ.DI;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyNetQConsole
{
    public static class EasyNetQServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEasyNetQ(this IServiceCollection serviceCollection, Func<IServiceResolver, ConnectionConfiguration> connectionConfigurationFactory, Action<IServiceRegister> registerServices)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var serviceRegister = new ServiceCollectionAdapter(serviceCollection);
            RabbitHutch.RegisterBus(serviceRegister, connectionConfigurationFactory, registerServices);
            return serviceCollection;
        }

        public static IServiceCollection RegisterEasyNetQ(this IServiceCollection serviceCollection, Func<IServiceResolver, ConnectionConfiguration> connectionConfigurationFactory)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection.RegisterEasyNetQ(connectionConfigurationFactory, c => { });
        }

        public static IServiceCollection RegisterEasyNetQ(this IServiceCollection serviceCollection, string connectionString, Action<IServiceRegister> registerServices)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection.RegisterEasyNetQ(c => c.Resolve<IConnectionStringParser>().Parse(connectionString), registerServices);
        }

        public static IServiceCollection RegisterEasyNetQ(this IServiceCollection serviceCollection, string connectionString)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection.RegisterEasyNetQ(c => c.Resolve<IConnectionStringParser>().Parse(connectionString));
        }
    }

    public class ServiceCollectionAdapter : IServiceRegister
    {
        private readonly IServiceCollection serviceCollection;

        public ServiceCollectionAdapter(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;

            this.serviceCollection.AddSingleton<IServiceResolver, ServiceProviderAdapter>();
        }

        public IServiceRegister Register<TService, TImplementation>(Lifetime lifetime = Lifetime.Singleton) where TService : class where TImplementation : class, TService
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    serviceCollection.AddTransient<TService, TImplementation>();
                    break;

                case Lifetime.Singleton:
                    serviceCollection.AddSingleton<TService, TImplementation>();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

            return this;
        }

        public IServiceRegister Register<TService>(TService instance) where TService : class
        {
            serviceCollection.AddSingleton(instance);
            return this;
        }

        public IServiceRegister Register<TService>(Func<IServiceResolver, TService> factory, Lifetime lifetime = Lifetime.Singleton) where TService : class
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    serviceCollection.AddTransient(x => factory(x.GetService<IServiceResolver>()));
                    break;

                case Lifetime.Singleton:
                    serviceCollection.AddSingleton(x => factory(x.GetService<IServiceResolver>()));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

            return this;
        }

        private class MicrosoftServiceResolverScope : IServiceResolverScope
        {
            private IServiceScope serviceScope;

            public MicrosoftServiceResolverScope(IServiceProvider serviceProvider)
            {
                serviceScope = serviceProvider.CreateScope();
            }

            public IServiceResolverScope CreateScope()
            {
                return new MicrosoftServiceResolverScope(serviceScope.ServiceProvider);
            }

            public void Dispose()
            {
                serviceScope?.Dispose();
            }

            public TService Resolve<TService>() where TService : class
            {
                return serviceScope.ServiceProvider.GetService<TService>();
            }
        }

        private class ServiceProviderAdapter : IServiceResolver
        {
            private readonly IServiceProvider serviceProvider;

            public ServiceProviderAdapter(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            public IServiceResolverScope CreateScope()
            {
                return new MicrosoftServiceResolverScope(serviceProvider);
            }

            public TService Resolve<TService>() where TService : class
            {
                return serviceProvider.GetService<TService>();
            }
        }
    }
}