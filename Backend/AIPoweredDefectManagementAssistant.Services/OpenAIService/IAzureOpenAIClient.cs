using AIPoweredDefectManagementAssistant.Models;

namespace AIPoweredDefectManagementAssistant.Services.OpenAIService
{
    public interface IAzureOpenAIClient
    {
        Task<ChatCompletionsResponseDto> SendChatCompletionsAsync(
            IEnumerable<ChatMessageDto> messages,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken cancellationToken = default);

        Task<string> GetAssistantFirstChoiceContentAsync(
            IEnumerable<ChatMessageDto> messages,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken cancellationToken = default);
    }
}