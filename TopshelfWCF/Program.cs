using Autofac;
using Autofac.Extras.NLog;
using Topshelf;
using Topshelf.Autofac;
using TopshelfWCF.Contracts.Services;

namespace TopshelfWCF {
    internal class Program {
        private static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterType<HelloWorldService>().As<IHelloWorldService>();
            builder.RegisterModule<NLogModule>();
            builder.RegisterType<HostService>();
            var container = builder.Build();

            HostFactory.Run(c => {
                c.UseAutofacContainer(container);
                c.Service<HostService>(s => {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });
        }
    }
}