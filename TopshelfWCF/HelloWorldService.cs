using TopshelfWCF.Contracts.Services;

namespace TopshelfWCF {
    public class HelloWorldService : IHelloWorldService {
        public string GetHello() {
            return "Hello world";
        }
    }
}