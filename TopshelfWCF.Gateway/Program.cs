using System;
using System.Collections.Specialized;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Extras.Quartz;
using Autofac.Integration.Wcf;
using Quartz;
using Topshelf;
using Topshelf.Autofac;
using Topshelf.Quartz;
using Topshelf.ServiceConfigurators;
using TopshelfWCF.Contracts.Services;

namespace TopshelfWCF.Gateway {
    public class Program {
        private static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterType<Service>();
            builder.RegisterModule<NLogModule>();
            RegisterWcfServices(builder);
            RegisterQuartz(builder);
            var container = builder.Build();

            ScheduleJobServiceConfiguratorExtensions.SchedulerFactory = () => container.Resolve<IScheduler>();

            HostFactory.Run(c => {
                c.UseLinuxIfAvailable();
                c.UseAutofacContainer(container);
                c.Service<Service>(s => {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                    ConfigureScheduler(s);
                });
            });
        }

        private static void ConfigureScheduler(ServiceConfigurator<Service> serviceConfigurator) {
            serviceConfigurator.ScheduleQuartzJob(q => {
                q.WithJob(JobBuilder.Create<Job>()
                    .WithIdentity("Heartbeat", "Maintenance")
                    .Build);
                q.AddTrigger(() => TriggerBuilder.Create()
                    .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(2)).Build());
            });
        }

        private static void RegisterQuartz(ContainerBuilder builder) {
            var schedulerConfig = new NameValueCollection {
                {"quartz.threadPool.threadCount", "3"},
                {"quartz.threadPool.threadNamePrefix", "SchedulerWorker"},
                {"quartz.scheduler.threadName", "Scheduler"}
            };

            builder.RegisterModule(new QuartzAutofacFactoryModule {
                ConfigurationProvider = c => schedulerConfig
            });
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(Job).Assembly));
        }

        private static void RegisterWcfServices(ContainerBuilder builder) {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            var endpoint = new EndpointAddress(new Uri("net.tcp://localhost:9091/HelloWorldService"));
            builder
                .Register(c => new ChannelFactory<IHelloWorldService>(
                    binding,
                    endpoint))
                .SingleInstance();
            builder
                .Register(c => {
                    var factory = c.Resolve<ChannelFactory<IHelloWorldService>>();
                    return factory.CreateChannel();
                })
                .As<IHelloWorldService>()
                .UseWcfSafeRelease();
        }
    }
}