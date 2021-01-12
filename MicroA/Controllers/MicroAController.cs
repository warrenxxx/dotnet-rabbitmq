using System.Threading.Tasks;
using MassTransit;
using MicroA.Command;
using MicroCore;
using MicroCore.Contracts;
using MicroCore.Contracts.MicroB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroAController : ControllerBase
    {
        private readonly IRequestClient<IMicroBRamCommand> _requestClient;
        private readonly ILogger<MicroAController> _logger;


        public MicroAController(IRequestClient<IMicroBRamCommand> requestClient, ILogger<MicroAController> logger)
        {
            _requestClient = requestClient;
            _logger = logger;
        }

        [HttpGet("" +
                 "{k:long}")]
        
        public async Task<string> Get(long k)
        {
            _logger.LogInformation("Request on A");
            return Utils.IncreaseCpu(k);
        }

        [HttpGet("ToMicroB/{k:long}")]
        public async Task<string> ToMicroB(long k)
        {
            var result = await _requestClient.GetResponse<MicroBRamResponse>(new MicroBRamCommand
            {
                K = k
            });
            return result.Message.Response;
        }
    }
}