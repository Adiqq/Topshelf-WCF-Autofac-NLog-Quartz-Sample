using Quartz;

namespace TopshelfWCF.Gateway {
    public class Service {
        private readonly IScheduler scheduler;

        public Service(IScheduler scheduler) {
            this.scheduler = scheduler;
        }

        public bool Start() {
            scheduler.Start();
            return true;
        }

        public bool Stop() {
            scheduler.Shutdown(true);
            return true;
        }
    }
}