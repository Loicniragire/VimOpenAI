using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgLeasing.System.Logging.Contract;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        #region Field
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;
        #endregion Field

        #region Constructor
        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }
        #endregion Constructor

        #region Action
        /// <summary>
        /// Uses the HealthCheckService to return the results in a customized body.
        /// </summary>
        /// <returns><see cref="HealthCheckResponse"/> with a 200 or 500 status code.</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Uses the HealthCheckService to return the results in a customized body.")]
        public async Task<IActionResult> HealthCheckAsync()
        {
            try
            {
                HealthReport report = await _healthCheckService.CheckHealthAsync();

                var response = new HealthCheckResponse
                {
                    Status = report.Status.ToString(),
                    HealthChecks = report.Entries.Select(entry => new SubHealthCheck
                    {
                        Component = entry.Key,
                        Status = entry.Value.Status.ToString(),
                        Results = entry.Value.Data
                    }),
                    HealthCheckDuration = (int)report.TotalDuration.TotalMilliseconds + "ms"
                };

                return report.Status == HealthStatus.Healthy ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occured performing the HealthCheck. Check this log for the exception details", exception: ex);
                return StatusCode(StatusCodes.Status500InternalServerError, HealthStatus.Unhealthy);
            }
        }
        #endregion Action
    }
}
