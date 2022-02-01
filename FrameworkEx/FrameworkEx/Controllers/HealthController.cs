using Microsoft.Extensions.Configuration;
using Serilog;
//using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FrameworkEx.Controllers
{
    [RoutePrefix("health")]
    public class HealthController : ApiController
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;

        public HealthController(ILogger log, IConfiguration config)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));

            _config = config
                ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpGet]
        [Route("v1")]
        public async Task<IHttpActionResult> GetV1()
        {
            _log.Information("Health Endpoint Called.");
            return Json(new 
            { 
                ApplicationName = _config[EnvironmentVariableConsts.ApplicationNameKey], 
                BuildNumber = _config[EnvironmentVariableConsts.BuildNumberKey] 
            });
        }
    }
}
