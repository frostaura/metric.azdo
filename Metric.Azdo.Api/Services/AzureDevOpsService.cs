using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Metric.Azdo.Api.Models;

namespace Metric.Azdo.Api.Services;

/// <summary>
/// Service for interacting with Azure DevOps and calculating DORA metrics
/// </summary>
public class AzureDevOpsService
{
    private readonly AzureDevOpsConfig _config;
    private readonly ILogger<AzureDevOpsService> _logger;
    private VssConnection? _connection;

    public AzureDevOpsService(IOptions<AzureDevOpsConfig> config, ILogger<AzureDevOpsService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Get connection to Azure DevOps
    /// </summary>
    private VssConnection GetConnection()
    {
        if (_connection == null)
        {
            var credentials = new VssBasicCredential(string.Empty, _config.PersonalAccessToken);
            _connection = new VssConnection(new Uri(_config.OrganizationUrl), credentials);
        }
        return _connection;
    }

    /// <summary>
    /// Get all projects in the organization
    /// </summary>
    public async Task<List<TeamProjectReference>> GetProjectsAsync()
    {
        try
        {
            var connection = GetConnection();
            var projectClient = connection.GetClient<ProjectHttpClient>();
            
            var projects = await projectClient.GetProjects();
            return projects.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects from Azure DevOps");
            throw;
        }
    }

    /// <summary>
    /// Calculate DORA metrics for a specific project
    /// </summary>
    public async Task<DoraMetrics> GetDoraMetricsAsync(string projectId, int periodDays = 30)
    {
        try
        {
            var connection = GetConnection();
            var projectClient = connection.GetClient<ProjectHttpClient>();
            var buildClient = connection.GetClient<BuildHttpClient>();

            // Get project details
            var project = await projectClient.GetProject(projectId);
            
            var metrics = new DoraMetrics
            {
                ProjectId = projectId,
                ProjectName = project.Name
            };

            // Calculate deployment frequency
            await CalculateDeploymentFrequency(buildClient, projectId, metrics, periodDays);
            
            // Calculate lead time for changes
            await CalculateLeadTimeForChanges(buildClient, projectId, metrics, periodDays);
            
            // Calculate change failure rate
            await CalculateChangeFailureRate(buildClient, projectId, metrics, periodDays);
            
            // Calculate time to recovery
            await CalculateTimeToRecovery(buildClient, projectId, metrics, periodDays);

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating DORA metrics for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Calculate deployment frequency
    /// </summary>
    private async Task CalculateDeploymentFrequency(BuildHttpClient buildClient, string projectId, DoraMetrics metrics, int periodDays)
    {
        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-periodDays);

            // Get all completed builds in the time period
            var builds = await buildClient.GetBuildsAsync(
                project: projectId,
                statusFilter: BuildStatus.Completed,
                resultFilter: BuildResult.Succeeded
            );

            // Filter builds by date and deployment criteria
            var recentBuilds = builds.Where(b => 
                b.FinishTime >= startDate && 
                b.FinishTime <= endDate &&
                (b.Definition.Name.ToLower().Contains("deploy") ||
                 b.Definition.Name.ToLower().Contains("release") ||
                 (b.Tags != null && b.Tags.Any(tag => tag.ToLower().Contains("deploy")))))
                .ToList();

            metrics.DeploymentFrequency.DeploymentsCount = recentBuilds.Count;
            metrics.DeploymentFrequency.PeriodDays = periodDays;

            _logger.LogInformation("Calculated deployment frequency: {Count} deployments in {Days} days for project {ProjectId}", 
                recentBuilds.Count, periodDays, projectId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not calculate deployment frequency for project {ProjectId}", projectId);
            // Set default values
            metrics.DeploymentFrequency.DeploymentsCount = 0;
            metrics.DeploymentFrequency.PeriodDays = periodDays;
        }
    }

    /// <summary>
    /// Calculate lead time for changes
    /// </summary>
    private async Task CalculateLeadTimeForChanges(BuildHttpClient buildClient, string projectId, DoraMetrics metrics, int periodDays)
    {
        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-periodDays);

            // Get recent successful deployment builds
            var builds = await buildClient.GetBuildsAsync(
                project: projectId,
                statusFilter: BuildStatus.Completed,
                resultFilter: BuildResult.Succeeded,
                top: 50 // Limit to recent builds for performance
            );

            var recentBuilds = builds.Where(b => 
                b.FinishTime >= startDate && 
                b.FinishTime <= endDate &&
                (b.Definition.Name.ToLower().Contains("deploy") ||
                 b.Definition.Name.ToLower().Contains("release")))
                .ToList();

            if (recentBuilds.Any())
            {
                var leadTimes = new List<double>();

                foreach (var build in recentBuilds.Take(20)) // Sample recent builds
                {
                    try
                    {
                        // For simplicity, we'll use build queue time to finish time as lead time
                        // In a real implementation, you'd get the commit time and calculate from there
                        var leadTimeHours = (build.FinishTime - build.QueueTime)?.TotalHours ?? 0;
                        if (leadTimeHours > 0)
                        {
                            leadTimes.Add(leadTimeHours);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not calculate lead time for build {BuildId}", build.Id);
                    }
                }

                if (leadTimes.Any())
                {
                    metrics.LeadTimeForChanges.AverageLeadTimeHours = leadTimes.Average();
                }
            }

            _logger.LogInformation("Calculated lead time for changes: {Hours} hours average for project {ProjectId}", 
                metrics.LeadTimeForChanges.AverageLeadTimeHours, projectId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not calculate lead time for changes for project {ProjectId}", projectId);
            // Set default value
            metrics.LeadTimeForChanges.AverageLeadTimeHours = 0;
        }
    }

    /// <summary>
    /// Calculate change failure rate
    /// </summary>
    private async Task CalculateChangeFailureRate(BuildHttpClient buildClient, string projectId, DoraMetrics metrics, int periodDays)
    {
        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-periodDays);

            // Get all deployment builds (both successful and failed) in the time period
            var allBuilds = await buildClient.GetBuildsAsync(
                project: projectId,
                statusFilter: BuildStatus.Completed
            );

            var deploymentBuilds = allBuilds.Where(b => 
                b.FinishTime >= startDate && 
                b.FinishTime <= endDate &&
                (b.Definition.Name.ToLower().Contains("deploy") ||
                 b.Definition.Name.ToLower().Contains("release")))
                .ToList();

            var failedBuilds = deploymentBuilds.Where(b => 
                b.Result == BuildResult.Failed ||
                b.Result == BuildResult.Canceled ||
                b.Result == BuildResult.PartiallySucceeded)
                .ToList();

            metrics.ChangeFailureRate.TotalDeployments = deploymentBuilds.Count;
            metrics.ChangeFailureRate.FailedDeployments = failedBuilds.Count;

            _logger.LogInformation("Calculated change failure rate: {Failed}/{Total} for project {ProjectId}", 
                failedBuilds.Count, deploymentBuilds.Count, projectId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not calculate change failure rate for project {ProjectId}", projectId);
            // Set default values
            metrics.ChangeFailureRate.TotalDeployments = 0;
            metrics.ChangeFailureRate.FailedDeployments = 0;
        }
    }

    /// <summary>
    /// Calculate time to recovery
    /// </summary>
    private async Task CalculateTimeToRecovery(BuildHttpClient buildClient, string projectId, DoraMetrics metrics, int periodDays)
    {
        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-periodDays);

            // Get deployment builds ordered by date
            var builds = await buildClient.GetBuildsAsync(
                project: projectId,
                statusFilter: BuildStatus.Completed
            );

            var deploymentBuilds = builds
                .Where(b => 
                    b.FinishTime >= startDate && 
                    b.FinishTime <= endDate &&
                    (b.Definition.Name.ToLower().Contains("deploy") ||
                     b.Definition.Name.ToLower().Contains("release")))
                .OrderBy(b => b.FinishTime)
                .ToList();

            var recoveryTimes = new List<double>();

            for (int i = 0; i < deploymentBuilds.Count - 1; i++)
            {
                var currentBuild = deploymentBuilds[i];
                var nextBuild = deploymentBuilds[i + 1];

                // If current build failed and next build succeeded, calculate recovery time
                if ((currentBuild.Result == BuildResult.Failed || 
                     currentBuild.Result == BuildResult.PartiallySucceeded) &&
                    nextBuild.Result == BuildResult.Succeeded)
                {
                    var recoveryTime = (nextBuild.FinishTime - currentBuild.FinishTime)?.TotalHours ?? 0;
                    if (recoveryTime > 0)
                    {
                        recoveryTimes.Add(recoveryTime);
                    }
                }
            }

            if (recoveryTimes.Any())
            {
                metrics.TimeToRecovery.AverageRecoveryTimeHours = recoveryTimes.Average();
            }

            _logger.LogInformation("Calculated time to recovery: {Hours} hours average for project {ProjectId}", 
                metrics.TimeToRecovery.AverageRecoveryTimeHours, projectId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not calculate time to recovery for project {ProjectId}", projectId);
            // Set default value
            metrics.TimeToRecovery.AverageRecoveryTimeHours = 0;
        }
    }

    /// <summary>
    /// Get DORA metrics for all projects
    /// </summary>
    public async Task<List<DoraMetrics>> GetAllProjectsDoraMetricsAsync(int periodDays = 30)
    {
        try
        {
            var projects = await GetProjectsAsync();
            var allMetrics = new List<DoraMetrics>();

            foreach (var project in projects)
            {
                try
                {
                    var metrics = await GetDoraMetricsAsync(project.Id.ToString(), periodDays);
                    allMetrics.Add(metrics);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not calculate metrics for project {ProjectName}", project.Name);
                    // Add empty metrics for this project
                    allMetrics.Add(new DoraMetrics 
                    { 
                        ProjectId = project.Id.ToString(), 
                        ProjectName = project.Name 
                    });
                }
            }

            return allMetrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DORA metrics for all projects");
            throw;
        }
    }
}