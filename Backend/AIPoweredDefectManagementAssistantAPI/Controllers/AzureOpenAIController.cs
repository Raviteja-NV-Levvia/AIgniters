using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;
using AIPoweredDefectManagementAssistant.Services.OpenAIService;
using Microsoft.AspNetCore.Mvc;

namespace AIPoweredDefectManagementAssistantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureOpenAIController : ControllerBase
    {
        private readonly IAzureOpenAIClient _azureClient;

        public AzureOpenAIController(IAzureOpenAIClient azureClient)
        {
            _azureClient = azureClient;
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "ok" });

        /// <summary>
        /// Send chat messages to Azure OpenAI and return the assistant's first choice content.
        /// Body: JSON array of ChatMessageDto (role/content).
        /// </summary>
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] IEnumerable<ChatMessageDto> messages, CancellationToken cancellationToken)
        {
            if (messages == null)
                return BadRequest(new { error = "messages body is required" });

            var content = await _azureClient.GetAssistantFirstChoiceContentAsync(messages, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok(new { content });
        }
    }
}