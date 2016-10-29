using System;
using System.ServiceModel;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.Wcf;
using TopshelfWCF.Contracts.Services;

namespace TopshelfWCF {
    public class HostService : IHostService {
        private readonly ILifetimeScope scope;
        private readonly ILogger logger;
        private ServiceHost host;

        public HostService(ILifetimeScope scope, ILogger logger) {
            this.scope = scope;
            this.logger = logger;
        }

        public bool Start() {
            host = new ServiceHost(typeof(HelloWorldService));
            host.AddDependencyInjectionBehavior<IHelloWorldService>(scope);
            host.Open();

            logger.Info("The host has been opened.");
            return true;
        }

        public bool Stop() {
            host.Close();
            logger.Info("Host closed");
            return true;
        }
    }
}