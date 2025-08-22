namespace Metric.Azdo.Api.Models;

/// <summary>
/// Azure DevOps configuration settings
/// </summary>
public class AzureDevOpsConfig
{
    /// <summary>
    /// Azure DevOps organization URL (e.g., https://dev.azure.com/yourorg)
    /// </summary>
    public string OrganizationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Personal Access Token for authentication
    /// </summary>
    public string PersonalAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Collection name (usually DefaultCollection)
    /// </summary>
    public string CollectionName { get; set; } = "DefaultCollection";
}