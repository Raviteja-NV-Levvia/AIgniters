using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;
using Microsoft.Extensions.Configuration;

namespace AIPoweredDefectManagementAssistant.Services.AzureService
{
    public class AzureServices : IAzureServices
    {
        private readonly string _azureUrl;
        private readonly string _token;
        private readonly string _project;
        private readonly IHttpClientFactory? _httpClientFactory;

        public AzureServices(IConfiguration configuration, IHttpClientFactory? httpClientFactory = null)
        {
            var settings = new AzureSettings();
            configuration.GetSection("AzureDevOps").Bind(settings);

            _azureUrl = settings.Url?.TrimEnd('/') ?? throw new ArgumentException("AzureDevOps:Url not configured in configuration.");
            _token = settings.Token ?? throw new ArgumentException("AzureDevOps:Token not configured in configuration.");
            _project = settings.Project ?? throw new ArgumentException("AzureDevOps:Project not configured in configuration.");

            _httpClientFactory = httpClientFactory;
        }

        public async Task<AzureCreateWorkItemResponseDto> CreateWorkItemAsync(AzureCreateWorkItemRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required to create a work item.", nameof(request.Title));

            var ops = new List<object>
            {
                new { op = "add", path = "/fields/System.Title", value = request.Title }
            };

            if (!string.IsNullOrWhiteSpace(request.Description))
                ops.Add(new { op = "add", path = "/fields/System.Description", value = request.Description });

            if (!string.IsNullOrWhiteSpace(request.AssignedTo))
                ops.Add(new { op = "add", path = "/fields/System.AssignedTo", value = request.AssignedTo });

            if (!string.IsNullOrWhiteSpace(request.AreaPath))
                ops.Add(new { op = "add", path = "/fields/System.AreaPath", value = request.AreaPath });

            if (!string.IsNullOrWhiteSpace(request.IterationPath))
                ops.Add(new { op = "add", path = "/fields/System.IterationPath", value = request.IterationPath });

            if (!string.IsNullOrWhiteSpace(request.Tags))
                ops.Add(new { op = "add", path = "/fields/System.Tags", value = request.Tags });

            if (request.AdditionalFields != null)
            {
                foreach (var kv in request.AdditionalFields)
                {
                    // Additional fields must be full referenceable field names, e.g. "Custom.MyField"
                    ops.Add(new { op = "add", path = $"/fields/{kv.Key}", value = kv.Value });
                }
            }

            using var http = _httpClientFactory?.CreateClient() ?? new HttpClient();

            // Build request URL:
            // Expecting _azureUrl like https://dev.azure.com/{organization}
            var workItemType = string.IsNullOrWhiteSpace(request.WorkItemType) ? "Bug" : request.WorkItemType;
            var requestUrl = $"{_azureUrl}/{_project}/_apis/wit/workitems/${workItemType}?api-version=7.1-preview.3";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            // Azure DevOps PAT uses Basic auth with empty username and PAT as password
            var basicToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_token}"));
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicToken);

            var payload = JsonSerializer.Serialize(ops);
            httpRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json-patch+json");

            var response = await http.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to create work item in Azure DevOps. Status: {response.StatusCode}. Response: {raw}");
            }

            // Parse response
            var doc = JsonSerializer.Deserialize<JsonElement>(raw);

            var result = new AzureCreateWorkItemResponseDto
            {
                RawResponse = raw,
                Fields = new Dictionary<string, object>()
            };

            if (doc.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number)
                result.Id = idProp.GetInt32();

            if (doc.TryGetProperty("url", out var urlProp) && urlProp.ValueKind == JsonValueKind.String)
                result.Url = urlProp.GetString() ?? result.Url;

            if (doc.TryGetProperty("fields", out var fieldsProp) && fieldsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var field in fieldsProp.EnumerateObject())
                {
                    try
                    {
                        result.Fields[field.Name] = field.Value.ValueKind switch
                        {
                            JsonValueKind.String => field.Value.GetString() ?? string.Empty,
                            JsonValueKind.Number => field.Value.GetDouble(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            _ => field.Value.ToString() ?? string.Empty
                        };
                    }
                    catch
                    {
                        result.Fields[field.Name] = field.Value.ToString() ?? string.Empty;
                    }
                }
            }

            return result;
        }
    }
}
