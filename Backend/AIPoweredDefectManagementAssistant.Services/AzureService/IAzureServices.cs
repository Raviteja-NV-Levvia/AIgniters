using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;

namespace AIPoweredDefectManagementAssistant.Services.AzureService
{
    public interface IAzureServices
    {
        /// <summary>
        /// Creates a work item (Bug/Defect or other type) in Azure DevOps.
        /// </summary>
        Task<AzureCreateWorkItemResponseDto> CreateWorkItemAsync(AzureCreateWorkItemRequestDto request, CancellationToken cancellationToken = default);
    }
}
