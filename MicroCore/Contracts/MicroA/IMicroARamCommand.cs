namespace MicroCore.Contracts.MicroA
{
    public interface IMicroARamCommand : IServiceBusCommand
    {
        public long K { get; set; } 
    }

    public class MicroARamResponse
    {
        public string Response { get; set; }
    }
}