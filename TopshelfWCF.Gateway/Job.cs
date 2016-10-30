using System;
using System.ServiceModel;
using NLog;
using Quartz;
using TopshelfWCF.Contracts.Services;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace TopshelfWCF.Gateway {
    public class Job : IJob {
        private readonly ILogger logger;
        private readonly IHelloWorldService service;

        public Job(IHelloWorldService service, ILogger logger) {
            this.service = service;
            this.logger = logger;
        }

        public void Execute(IJobExecutionContext context) {
            try {
                logger.Info(service.GetHello());
            }
            catch (EndpointNotFoundException e) {
                logger.LogException(LogLevel.Error, "Server is not responding", e);
            }
            catch (Exception e) {
                logger.LogException(LogLevel.Fatal, e.Message, e);
            }
        }
    }
}