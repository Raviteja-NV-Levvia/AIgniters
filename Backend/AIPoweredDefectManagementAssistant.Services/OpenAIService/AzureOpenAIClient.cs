using System.Text;
using System.Text.Json;
using AIPoweredDefectManagementAssistant.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;
using Microsoft.Extensions.Configuration;

namespace AIPoweredDefectManagementAssistant.Services.OpenAIService
{
    /// <summary>
    /// Generic client to call Azure OpenAI Chat Completions (deployment-based).
    /// Focused on Azure OpenAI: uses deployment in the URL and the "api-key" header.
    /// Now reads configuration values directly from IConfiguration instead of binding OpenAISettings.
    /// </summary>
    public class AzureOpenAIClient : IAzureOpenAIClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory? _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public AzureOpenAIClient(IConfiguration configuration, IHttpClientFactory? httpClientFactory = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory;

            var useAzure = _configuration.GetValue<bool>("OpenAI:UseAzure");
            if (!useAzure)
                throw new ArgumentException("OpenAI:UseAzure must be true for AzureOpenAIClient.");

            var endpoint = _configuration.GetValue<string>("OpenAI:AzureEndpoint");
            var apiKey = _configuration.GetValue<string>("OpenAI:AzureApiKey");
            var deployment = _configuration.GetValue<string>("OpenAI:AzureDeploymentName");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("OpenAI:AzureEndpoint is not configured.");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("OpenAI:AzureApiKey is not configured.");
            if (string.IsNullOrWhiteSpace(deployment))
                throw new ArgumentException("OpenAI:AzureDeploymentName is not configured.");
        }

        /// <summary>
        /// Sends a Chat Completions request to the Azure OpenAI deployment and returns the deserialized response DTO.
        /// </summary>
        public async Task<ChatCompletionsResponseDto> SendChatCompletionsAsync(
            IEnumerable<ChatMessageDto> messages,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken cancellationToken = default)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            using var http = _httpClientFactory?.CreateClient() ?? new HttpClient();

            var baseUrl = (_configuration.GetValue<string>("OpenAI:AzureEndpoint") ?? string.Empty).TrimEnd('/');
            var deploymentName = _configuration.GetValue<string>("OpenAI:AzureDeploymentName") ?? throw new InvalidOperationException("OpenAI:AzureDeploymentName not configured.");
            var apiVersion = _configuration.GetValue<string>("OpenAI:AzureApiVersion") ?? "2023-05-15";
            var apiKey = _configuration.GetValue<string>("OpenAI:AzureApiKey") ?? throw new InvalidOperationException("OpenAI:AzureApiKey not configured.");

            var endpoint = $"{baseUrl}/openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("api-key", apiKey);

            // read defaults from configuration when callers don't pass overrides
            var cfgTemperature = _configuration.GetValue<double?>("OpenAI:Temperature") ?? 0.2;
            var cfgMaxTokens = _configuration.GetValue<int?>("OpenAI:MaxTokens") ?? 800;

            var chatRequest = new ChatCompletionsRequestDto
            {
                // For Azure we do NOT include model in the payload because the deployment is specified in the URL
                Model = null,
                Temperature = temperature ?? cfgTemperature,
                MaxTokens = maxTokens ?? cfgMaxTokens
            };

            foreach (var m in messages)
                chatRequest.Messages.Add(m);

            var payload = JsonSerializer.Serialize(chatRequest, _jsonOptions);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            using var resp = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var raw = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                // bubble up status and body for easier diagnostics
                throw new InvalidOperationException($"Azure OpenAI request failed: {resp.StatusCode}. Response: {raw}");
            }

            ChatCompletionsResponseDto? chatResponse;
            try
            {
                chatResponse = JsonSerializer.Deserialize<ChatCompletionsResponseDto>(raw, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize Azure OpenAI response.", ex);
            }

            if (chatResponse == null)
                throw new InvalidOperationException("Azure OpenAI response deserialized to null.");

            return chatResponse;
        }

        /// <summary>
        /// Convenience method that returns the assistant content (first choice) as plain text.
        /// </summary>
        public async Task<string> GetAssistantFirstChoiceContentAsync(
            IEnumerable<ChatMessageDto> messages,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken cancellationToken = default)
        {
            var resp = await SendChatCompletionsAsync(messages, temperature, maxTokens, cancellationToken).ConfigureAwait(false);

            if (resp.Choices == null || resp.Choices.Count == 0)
                throw new InvalidOperationException("Azure OpenAI returned no choices.");

            var content = resp.Choices[0].Message?.Content ?? string.Empty;
            return content;
        }
    }
}