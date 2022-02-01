using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreEx.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HealthController> _log;

        public HealthController(IConfiguration config, ILogger<HealthController> log)
        {
            _config = config
                ?? throw new ArgumentNullException(nameof(config));

            _log = log
                ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        /// This is a sample endpoint to use for testing.
        /// </summary>
        /// <returns>Sample Json Object</returns>
        [HttpGet("v1")]
        [Produces("application/json")]
        public async Task<IActionResult> GetV1()
        {
            _log.LogInformation("Health Endpoint hit.");
            return Ok(await Task.FromResult(new
            {
                ApplicationName = _config[EnvironmentVariableConsts.ApplicationNameKey],
                BuildNumber = _config[EnvironmentVariableConsts.BuildNumberKey]
            }));
        }
    }
}
