using System.Threading.Tasks;
using MassTransit;
using MicroCore;
using MicroCore.Contracts.MicroA;
using Microsoft.Extensions.Logging;

namespace MicroA.Consumers
{
    public class MicroARamConsumer : IConsumer<IMicroARamCommand>
    {
        private readonly ILogger<MicroARamConsumer> _logger;

        public MicroARamConsumer(ILogger<MicroARamConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IMicroARamCommand> context)
        {
            _logger.LogInformation("Ram consumer in A");
            await context.RespondAsync(new MicroARamResponse()
            {
                Response = Utils.IncreaseCpu(context.Message.K)
            });
        }
    }
}