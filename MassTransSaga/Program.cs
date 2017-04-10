using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransSaga
{
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Threading;

    using Autofac;

    using Automatonymous;

    using GreenPipes;

    using MassTransit;
    using MassTransit.EntityFrameworkIntegration;
    using MassTransit.EntityFrameworkIntegration.Saga;
    using MassTransit.NLogIntegration;
    using MassTransit.QuartzIntegration;
    using MassTransit.QuartzIntegration.Configuration;
    using MassTransit.Saga;
    using MassTransit.Scheduling;

    using MassTransSaga.Datastore.Context;
    using MassTransSaga.Model;

    using Quartz;
    using Quartz.Impl;

    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            var builder = new ContainerBuilder();
            builder.RegisterType<TheGreatSaga>().AsSelf().SingleInstance();

            builder.Register(
                    context =>
                        {
                            SagaDbContextFactory factory = () => new TheGreatSagaDbContext();
                            return new EntityFrameworkSagaRepository<TheGreatState>(factory, optimistic: true);
                        }).As<ISagaRepository<TheGreatState>>()
                .SingleInstance();
            
            builder.Register(
                context =>
                    {
                        var busControl = Bus.Factory.CreateUsingRabbitMq(
                            cfg =>
                                {
                                    cfg.UseNLog();
                                    var host = cfg.Host(
                                        new Uri("rabbitmq://localhost/"),
                                        h =>
                                            {
                                                h.Username("guest");
                                                h.Password("guest");
                                            });

                                    cfg.ReceiveEndpoint(
                                        host,
                                        "Wiggly.Wombat.TheGreatSaga",
                                        epc =>
                                            {
                                                epc.UseInMemoryOutbox();
                                                epc.UseRetry(
                                                    x =>
                                                        {
                                                            x.Handle<DbUpdateConcurrencyException>();
                                                            x.Interval(5, TimeSpan.FromMilliseconds(100));
                                                        }); // Add Retry Middleware for Optimistic Concurrency

                                                var repository = context.Resolve<ISagaRepository<TheGreatState>>();
                                                var saga = context.Resolve<TheGreatSaga>();

                                                ushort concurrency = 8;
                                                epc.PrefetchCount = (ushort)(concurrency * 2);

                                                var partitioner = epc.CreatePartitioner(concurrency);
                                                epc.StateMachineSaga(
                                                    saga,
                                                    repository,
                                                    sc =>
                                                        {
                                                            sc.Message<IUpdateGreatState>(
                                                                m => m.UsePartitioner(
                                                                    partitioner,
                                                                    c => c.Message.EventId));
                                                        });
                                            });

                                });
                        return busControl;
                    }).SingleInstance().As<IBusControl>().As<IBus>();


            var container = builder.Build();

            var control = container.Resolve<IBusControl>();

            control.StartAsync().Wait();
        }
    }
}
