using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLeasing.System.Logging.Contract;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Controllers
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [Route("JITDecision/")]
    [ApiController]
    public class JITDecisionController : ControllerBase
    {
        private readonly ILogger<JITDecisionController> _logger;
        private readonly IJITDecisionProvider _provider;

        public JITDecisionController(ILogger<JITDecisionController> logger, IJITDecisionProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JITFundingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Approves or denies just in time (JIT) funding authorization for a virtual card.")]
        public async Task<IActionResult> Post(JITFundingRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, "JITDecision endpoint has been called");
                var response = await _provider.ProcessJITDecisionAsync(request);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (HttpRequestException ex)
            {
                var responseCode = ex.StatusCode ?? System.Net.HttpStatusCode.InternalServerError;
                return Problem(title: ex.Message, statusCode: (int)responseCode);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occurred while processing JIT funding request", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
