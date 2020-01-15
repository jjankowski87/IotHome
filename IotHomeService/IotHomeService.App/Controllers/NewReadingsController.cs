using System;
using System.Threading.Tasks;
using IotHomeService.App.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IotHomeService.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewReadingsController : ControllerBase
    {
        private readonly IReadingsNotifier _readingsNotifier;

        public NewReadingsController(IReadingsNotifier readingsNotifier)
        {
            _readingsNotifier = readingsNotifier;
        }

        [HttpPost("{sensor}")]
        public async Task<ActionResult> Post(string sensor, [FromQuery]string name, [FromQuery]DateTimeOffset date, [FromQuery]decimal value)
        {
            await _readingsNotifier.NotifyClientsAsync(sensor, name, date, value);

            return new OkResult();
        }
    }
}
