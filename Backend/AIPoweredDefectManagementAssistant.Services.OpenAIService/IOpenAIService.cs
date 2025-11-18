using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;

namespace AIPoweredDefectManagementAssistant.Services.OpenAIService
{
    public interface IOpenAIService
    {
        /// <summary>
        /// Call OpenAI with provided defect information and return a mapped AzureCreateWorkItemRequestDto.
        /// </summary>
        Task<AzureCreateWorkItemRequestDto> GenerateAzureWorkItemAsync(OpenAIRequestDto request, CancellationToken cancellationToken = default);
    }
}