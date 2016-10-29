using System.ServiceModel;

namespace TopshelfWCF.Contracts.Services {
    [ServiceContract]
    public interface IHelloWorldService {
        [OperationContract]
        string GetHello();
    }
}