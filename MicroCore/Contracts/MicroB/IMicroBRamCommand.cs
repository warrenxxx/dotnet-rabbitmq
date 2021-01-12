namespace MicroCore.Contracts.MicroB
{
    public interface IMicroBRamCommand : IServiceBusCommand
    {
        public long K { get; set; }
    }
    public class MicroBRamResponse
    {
        public string Response { get; set; }
    }
}