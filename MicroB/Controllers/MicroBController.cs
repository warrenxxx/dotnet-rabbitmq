using System.Threading.Tasks;
using MassTransit;
using MicroB.Command;
using MicroCore;
using MicroCore.Contracts.MicroA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroBController : ControllerBase
    {
        private readonly IRequestClient<IMicroARamCommand> _requestClient;
        private readonly ILogger<MicroBController> _logger;

        public MicroBController(IRequestClient<IMicroARamCommand> requestClient, ILogger<MicroBController> logger)
        {
            _requestClient = requestClient;
            _logger = logger;
        }

        [HttpGet("{k:long}")]
        public string Get(long k)
        {
            _logger.LogInformation("Request on A");
            return Utils.IncreaseCpu(k);
        }

        [HttpGet("ToMicroA/{k:long}")]
        public async Task<string> ToMicroA(long k)
        {
            var result = await _requestClient.GetResponse<MicroARamResponse>(new MicroARamCommand
            {
                K = k
            });
            return result.Message.Response;
        }
    }
}