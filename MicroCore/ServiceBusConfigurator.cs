using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MicroCore
{
    public static class Util
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source is null || !source.Any();
        }
    }


    public class ServiceBusConfigurator
    {
        private readonly IServiceCollection services;
        private readonly List<Type> consumers;
        private readonly List<Type> commands;

        public ServiceBusConfigurator(IServiceCollection services, Type[] scanConsumerTypes = null)
        {
            this.services = services;
            var scanConsumerAssemblies = scanConsumerTypes.IsNullOrEmpty() ? AppDomain.CurrentDomain.GetAssemblies() : 
                scanConsumerTypes.Select(t => t.Assembly).ToArray();
            consumers = ScanConsumers(scanConsumerAssemblies).ToList();
            commands = ScanCommands().ToList();
        }

        public void Configure()
        {
            // Begin service Bus configuration
            services.AddMassTransit(x =>
            {
                // Creates the IBus reference, configures JSON serialization,
                // registers receiving endpoints, and maps command types to addresses
                x.AddBus(provider => ConfigureBusControl(provider));
                x.Collection.AddMassTransitHostedService();

                OnMassTransitAddConsumers(x);

                //Add request/response clients
                OnMassTransitAddRequestClients(x);
            });

            //END Service Bus Config
        }

        private IBusControl ConfigureBusControl(IServiceProvider provider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ConfigureJsonSerializer(settings =>
                {
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    return settings;
                });
                cfg.ConfigureJsonDeserializer(settings =>
                {
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    return settings;
                });
                var host = cfg.Host("localhost");
                MapEndpointConventionsForRequestClients();
                ConfigureEndPointForConsumers(cfg, provider);

                var logger = provider.GetRequiredService<ILogger<ServiceBusConfigurator>>();

                if (consumers.Count==0)
                {
                    logger.LogInformation("There is no consumer was registered in this service.");
                }
            });
        }

        private void OnMassTransitAddRequestClients(IServiceCollectionConfigurator x)
        {
            if (!commands.IsNullOrEmpty())
            {
                MethodInfo addMethod = typeof(IRegistrationConfigurator).GetMethod("AddRequestClient",
                    new Type[] {typeof(Uri), typeof(RequestTimeout)});
                foreach (var cmd in commands)
                {
                    MethodInfo genericMethod = addMethod.MakeGenericMethod(cmd);
                    genericMethod.Invoke(x, new object[] {new Uri("rabbitmq://" + "localhost"+ "/" + cmd.FullName), null});
                }

                x.Collection.Decorate(typeof(IRequestClient<>), typeof(MultipleLenderRequestClient<>));
            }
        }

        private void MapEndpointConventionsForRequestClients()
        {
            MethodInfo mapMethod = typeof(EndpointConvention).GetMethod("Map", new Type[] {typeof(Uri)});
            foreach (var item in commands)
            {
                MethodInfo genericMethod = mapMethod.MakeGenericMethod(item);
                genericMethod.Invoke(null, new object[]
                {
                    new Uri("rabbitmq://" + "localhost"+ "/" + item.FullName)
                    
                });
            }
        }

        private void OnMassTransitAddConsumers(IServiceCollectionConfigurator x)
        {
            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    var iconsumer = consumer.GetInterface(typeof(IConsumer<>).Name);
                    x.Collection.AddScoped(iconsumer, consumer);
                    x.AddConsumer(consumer);
                }

                x.Collection.Decorate(typeof(IConsumer<>), typeof(ConsumerProxy<>));
            }
        }

        public void ConfigureEndPointForConsumers(IRabbitMqBusFactoryConfigurator cfg, IServiceProvider provider)
        {
            if (!consumers.IsNullOrEmpty())
            {
                MethodInfo consumerMethod =
                    typeof(ServiceBusConfigurator).GetMethod("OnConfigureReceiveEndpointForConsumer");
                foreach (var consumer in consumers)
                {
                    var iconsumer = consumer.GetInterface(typeof(IConsumer<>).Name);
                    Type[] typeArgs = iconsumer.GetGenericArguments();
                    Type commandType = typeArgs[0];
                    string endpoint = commandType.FullName;
                    cfg.ReceiveEndpoint(endpoint, e =>
                    {
                        MethodInfo genericMethod = consumerMethod.MakeGenericMethod(iconsumer);
                        genericMethod.Invoke(null, new object[] {e, provider});
                    });
                }
            }
        }

        /// <summary>
        /// Reflaction method. Do not delete it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rec"></param>
        /// <param name="provider"></param>
        public static void OnConfigureReceiveEndpointForConsumer<T>(IReceiveEndpointConfigurator rec,
            IServiceProvider provider) where T : class, IConsumer
        {
            static void configure(IConsumerConfigurator<T> cc)
            {
            }

            DependencyInjectionReceiveEndpointExtensions.Consumer(rec, provider,
                (Action<IConsumerConfigurator<T>>) configure);
            EndpointConvention.Map<T>(rec.InputAddress);
        }


        private IEnumerable<Type> ScanConsumers(Assembly[] scanConsumerAssemblies)
        {
            var consumerTypeName = typeof(IConsumer<>).Name;
            var types = scanConsumerAssemblies
                .SelectMany(s =>
                {
                    try
                    {
                        return s.GetTypes();
                    }
                    catch
                    {
                        return new Type[0];
                    }
                })
                .Where(p => p.GetInterfaces().Any(itf => itf.IsGenericType && itf.Name == consumerTypeName)
                            && p.Namespace.StartsWith("Micro")
                )
                .Where(p => p != typeof(ConsumerProxy<>))
                .ToList();
            return types;
        }

        private IEnumerable<Type> ScanCommands()
        {
            var type = typeof(IServiceBusCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s =>
                {
                    try
                    {
                        return s.GetTypes();
                    }
                    catch
                    {
                        return new Type[0];
                    }
                })
                .Where(p => p != type  && p.IsInterface && type.IsAssignableFrom(p))
                .Where(p => p.Namespace.StartsWith("Micro"));
            return types;
        }
    }
    public interface IServiceBusCommand
    {
        // Marker interface
    }
}