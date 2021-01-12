using System.Threading.Tasks;
using MassTransit;
using MicroCore;
using MicroCore.Contracts.MicroB;
using Microsoft.Extensions.Logging;

namespace MicroB.Consumers
{
    public class MicroBRamConsumer : IConsumer<IMicroBRamCommand>
    {
        private readonly ILogger<MicroBRamConsumer> _logger;

        public MicroBRamConsumer(ILogger<MicroBRamConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IMicroBRamCommand> context)
        {
            _logger.LogInformation("Ram consumer in B");
            await context.RespondAsync(new MicroBRamResponse() 
            {
                Response = Utils.IncreaseCpu(context.Message.K)
            });
        }
    }
}