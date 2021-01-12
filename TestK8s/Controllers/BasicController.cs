using Microsoft.AspNetCore.Mvc;
using TestK8s.Services;

namespace TestK8s.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasicController : ControllerBase
    {
        private readonly BasicService _basicService;

        public BasicController(BasicService basicService)
        {
            _basicService = basicService;
        }
        
        [HttpGet]
        public string Get()
        {
            return "Hello World " + _basicService.GetK();
        }
        
        
    }
}