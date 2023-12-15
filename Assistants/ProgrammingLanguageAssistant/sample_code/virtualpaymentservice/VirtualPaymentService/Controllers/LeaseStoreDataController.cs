using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLeasing.System.Logging.Contract;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Controllers
{
    [Route("LeaseStoreData/")]
    [ApiController]
    [Obsolete("This is a temporary solution for pulling lease/store data until VPO is able to provide it. Remove as soon as possible.")]
    public class LeaseStoreDataController : ControllerBase
    {
        private readonly ILogger<LeaseStoreDataController> _logger;
        private readonly ILeaseStoreDataProvider _provider;

        public LeaseStoreDataController(ILogger<LeaseStoreDataController> logger, ILeaseStoreDataProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LeaseStoreDataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] LeaseStoreDataRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, "LeaseStoreData endpoint has been called");
                var response = await Task.Run(() => _provider.GetLeaseStoreData(request));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occurred while fetching Lease/Store data", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("MarqetaStateId")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MarqetaStateResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMarqetaStateId(int stateId)
        {
            try
            {
                _logger.Log(LogLevel.Info, "LeaseStoreData/MarqetaStateId endpoint has been called");
                var response = await Task.Run(() => _provider.GetMarqetaStateAbbreviation(stateId));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occurred while fetching state abbreviation", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get all custom store defined fields for an associated lease
        /// </summary>
        /// <returns><see cref="IEnumerable{StoreCustomField}"/> with a 200 or 500 status code.</returns>
        [HttpGet]
        [Route("custom/{leaseId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<StoreCustomField>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Get all custom store defined fields for an associated lease")]
        public async Task<IActionResult> GetStoreCustomFields([FromRoute] int leaseId)
        {
            try
            {
                var vCards = await _provider.GetStoreCustomFields(leaseId);

                return Ok(vCards);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    $"An exception occurred while retrieving store custom fields for {nameof(leaseId)} {leaseId}. See the exception in this log for more details.",
                    exception: ex);
                return Problem(
                    $"An error occurred while attempting to retrieve store custom fields for {nameof(leaseId)} {leaseId}");
            }
        }
    }
}
