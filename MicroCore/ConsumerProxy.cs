using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MicroCore
{
    public class ConsumerProxy<T> : IConsumer<T> where T : class
    {
        private readonly IConsumer<T> _inner;
        private readonly ILogger<ConsumerProxy<T>> _logger;

        public ConsumerProxy(IConsumer<T> inner, ILogger<ConsumerProxy<T>> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            await _inner.Consume(context);
        }
    }
}