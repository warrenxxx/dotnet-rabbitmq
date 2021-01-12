using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using MicroCore.Contracts;
using Microsoft.Extensions.Logging;

namespace MicroB.Consumers
{
    public class HelloConsumer : IConsumer<IHelloFromCoreCommand>
    {
        private readonly ILogger<HelloConsumer> _logger;

        public HelloConsumer(ILogger<HelloConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IHelloFromCoreCommand> context)
        {
            _logger.LogInformation("Hello Consumer in B");
            await context.RespondAsync(new HelloFromCoreCommandResponse
            {
                HelloFromCoreCommandResponseResult = "Hello from B"
            });
        }
    }
}