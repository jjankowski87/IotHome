using IotHomeService.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace IotHomeService.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateMetricsController : ControllerBase
    {
        private readonly IotHubListener _iotHubListener;

        public UpdateMetricsController(IotHubListener iotHubListener)
        {
            _iotHubListener = iotHubListener;
        }

        [HttpGet]
        public ActionResult Post()
        {
            return new OkResult();
        }
    }
}
