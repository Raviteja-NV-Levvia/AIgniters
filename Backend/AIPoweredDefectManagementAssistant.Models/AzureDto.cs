using System;
using System.Collections.Generic;

namespace AIPoweredDefectManagementAssistant.Models
{
    // Request DTO to create a work item (bug/defect) in Azure DevOps
    public class AzureCreateWorkItemRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? AssignedTo { get; set; }
        public string WorkItemType { get; set; } = "Bug"; // e.g. Bug, Task
        public string? AreaPath { get; set; }
        public string? IterationPath { get; set; }
        public string? Tags { get; set; } // comma separated
        public Dictionary<string, object>? AdditionalFields { get; set; }
    }

    // Response DTO returned after creating a work item
    public class AzureCreateWorkItemResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, object>? Fields { get; set; }
        public string RawResponse { get; set; } = string.Empty;
    }

    // Configuration model to bind Azure settings from appsettings.json
    public class AzureSettings
    {
        /// <summary>
        /// Base Azure DevOps URL (e.g. https://dev.azure.com/{organization})
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Personal Access Token (PAT)
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Project name in Azure DevOps
        /// </summary>
        public string Project { get; set; } = string.Empty;
    }
}