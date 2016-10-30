using System.Collections.Specialized;
using System.ServiceModel;
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
            builder
                .Register(c => new ChannelFactory<IHelloWorldService>(
                    new BasicHttpBinding(),
                    new EndpointAddress("http://localhost:8080/HelloWorldService")))
                .SingleInstance();
            builder
                .Register(c => c.Resolve<ChannelFactory<IHelloWorldService>>().CreateChannel())
                .As<IHelloWorldService>()
                .UseWcfSafeRelease();
        }
    }
}