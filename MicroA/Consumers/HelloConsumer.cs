// using System.Threading.Tasks;
// using MassTransit;
// using MicroA.Command;
// using MicroCore.Contracts;
// using Microsoft.Extensions.Logging;
//
// namespace MicroA.Consumers
// {
//     public class HelloConsumer : IConsumer<IHelloFromCoreCommand>
//     {
//         private readonly ILogger<HelloConsumer> _logger;
//
//         public HelloConsumer(ILogger<HelloConsumer> logger)
//         {
//             _logger = logger;
//         }
//
//         public async Task Consume(ConsumeContext<IHelloFromCoreCommand> context)
//         {
//             _logger.LogInformation("Hello consumer in A");
//             await context.RespondAsync(new HelloFromCoreCommandResponse
//             {
//                 HelloFromCoreCommandResponseResult = "Hello from A"
//             });
//         }
//     }
// }