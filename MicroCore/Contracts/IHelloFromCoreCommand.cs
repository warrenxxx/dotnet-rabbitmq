namespace MicroCore.Contracts
{
    public interface IHelloFromCoreCommand : IServiceBusCommand
    {
    }

    public class HelloFromCoreCommandResponse
    {
        public string HelloFromCoreCommandResponseResult { get; set; }
    }
}