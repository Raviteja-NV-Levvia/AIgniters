using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;
using Microsoft.Extensions.Configuration;

namespace AIPoweredDefectManagementAssistant.Services.OpenAIService
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAISettings _settings;
        private readonly IHttpClientFactory? _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OpenAIService(IConfiguration configuration, IHttpClientFactory? httpClientFactory = null)
        {
            _httpClientFactory = httpClientFactory;
            _settings = new OpenAISettings();
            configuration.GetSection("OpenAI").Bind(_settings);

            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new ArgumentException("OpenAI:ApiKey not configured in configuration.");
        }

        public async Task<AzureCreateWorkItemRequestDto> GenerateAzureWorkItemAsync(OpenAIRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description is required.", nameof(request.Description));

            using var http = _httpClientFactory?.CreateClient() ?? new HttpClient();

            var baseUrl = _settings.BaseUrl?.TrimEnd('/') ?? "https://api.openai.com";
            var endpoint = $"{baseUrl}/v1/chat/completions";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

            // Build the instruction prompt. Ask the model to output a JSON object ONLY with specific fields.
            var systemPrompt = "You are a helpful assistant that converts a textual defect report into structured bug fields. " +
                                "Return a single JSON object with the following keys: title, description, reproducible_steps, severity, tags. " +
                                "severity should be one of: Critical, High, Medium, Low. Keep values succinct. Do not include any extra commentary.";

            var userPromptBuilder = new StringBuilder();
            userPromptBuilder.AppendLine("Raw defect information:");
            userPromptBuilder.AppendLine($"Description: {request.Description}");
            if (!string.IsNullOrWhiteSpace(request.StepsToReproduce))
                userPromptBuilder.AppendLine($"StepsToReproduce: {request.StepsToReproduce}");
            if (!string.IsNullOrWhiteSpace(request.Environment))
                userPromptBuilder.AppendLine($"Environment: {request.Environment}");
            if (!string.IsNullOrWhiteSpace(request.Logs))
                userPromptBuilder.AppendLine($"Logs: {request.Logs}");
            if (!string.IsNullOrWhiteSpace(request.AdditionalContext))
                userPromptBuilder.AppendLine($"AdditionalContext: {request.AdditionalContext}");

            userPromptBuilder.AppendLine();
            userPromptBuilder.AppendLine("Output JSON with keys exactly: title, description, reproducible_steps, severity, tags (tags as comma-separated).");

            var payloadObj = new
            {
                model = _settings.Model,
                temperature = _settings.Temperature,
                max_tokens = _settings.MaxTokens,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPromptBuilder.ToString() }
                }
            };

            var payload = JsonSerializer.Serialize(payloadObj, _jsonOptions);
            httpRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var resp = await http.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var raw = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"OpenAI request failed: {resp.StatusCode}. Response: {raw}");
            }

            // Parse assistant message content
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            var contentText = string.Empty;
            try
            {
                // choices[0].message.content
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var content))
                        contentText = content.GetString() ?? string.Empty;
                }
            }
            catch
            {
                contentText = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(contentText))
                throw new InvalidOperationException("OpenAI returned empty content.");

            // Attempt to extract first JSON object from the response (handles markdown/code fences)
            var jsonSegment = ExtractFirstJsonObject(contentText);
            if (string.IsNullOrWhiteSpace(jsonSegment))
                throw new InvalidOperationException("Could not extract JSON object from OpenAI response.");

            OpenAIResponseDto? parsed;
            try
            {
                parsed = JsonSerializer.Deserialize<OpenAIResponseDto>(jsonSegment, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize OpenAI response JSON.", ex);
            }

            if (parsed == null)
                throw new InvalidOperationException("OpenAI response parsed to null.");

            // Map to Azure DTO
            var azureRequest = new AzureCreateWorkItemRequestDto
            {
                Title = parsed.Title ?? parsed.Description?.Split('\n').FirstOrDefault() ?? "Auto-generated bug",
                Description = BuildAzureDescription(parsed, request),
                WorkItemType = string.IsNullOrWhiteSpace(request.WorkItemType) ? "Bug" : request.WorkItemType,
                AssignedTo = request.AssignedTo,
                Tags = parsed.Tags ?? request.Tags
            };

            if (!string.IsNullOrWhiteSpace(parsed.Severity))
            {
                azureRequest.AdditionalFields = azureRequest.AdditionalFields ?? new System.Collections.Generic.Dictionary<string, object>();
                // Map severity to a generic field named "Severity". Adjust to your organization field name if needed.
                azureRequest.AdditionalFields["Severity"] = parsed.Severity;
            }

            if (parsed.AdditionalFields != null)
            {
                azureRequest.AdditionalFields ??= new System.Collections.Generic.Dictionary<string, object>();
                foreach (var kv in parsed.AdditionalFields)
                    azureRequest.AdditionalFields[kv.Key] = kv.Value;
            }

            return azureRequest;
        }

        private static string BuildAzureDescription(OpenAIResponseDto parsed, OpenAIRequestDto original)
        {
            var sb = new StringBuilder();
            sb.AppendLine(parsed.Description ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(parsed.ReproducibleSteps))
            {
                sb.AppendLine();
                sb.AppendLine("Reproducible Steps:");
                sb.AppendLine(parsed.ReproducibleSteps);
            }
            if (!string.IsNullOrWhiteSpace(original.Environment))
            {
                sb.AppendLine();
                sb.AppendLine("Environment:");
                sb.AppendLine(original.Environment);
            }
            if (!string.IsNullOrWhiteSpace(original.Logs))
            {
                sb.AppendLine();
                sb.AppendLine("Logs:");
                sb.AppendLine(original.Logs);
            }
            return sb.ToString();
        }

        private static string? ExtractFirstJsonObject(string text)
        {
            // naive but practical: find first '{' and corresponding last '}' after it
            var start = text.IndexOf('{');
            if (start < 0) return null;

            // attempt to find matching brace by scanning
            int depth = 0;
            for (int i = start; i < text.Length; i++)
            {
                if (text[i] == '{') depth++;
                else if (text[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        var segment = text.Substring(start, i - start + 1);
                        return segment.Trim();
                    }
                }
            }

            // fallback: try to clean up markdown fences and parse inside
            var cleaned = text.Replace("```json", "").Replace("```", "").Trim();
            var cleanedStart = cleaned.IndexOf('{');
            if (cleanedStart >= 0)
            {
                var last = cleaned.LastIndexOf('}');
                if (last > cleanedStart)
                    return cleaned.Substring(cleanedStart, last - cleanedStart + 1).Trim();
            }

            return null;
        }
    }
}