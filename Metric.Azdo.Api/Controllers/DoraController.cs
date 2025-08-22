using Microsoft.AspNetCore.Mvc;
using Metric.Azdo.Api.Models;
using Metric.Azdo.Api.Services;

namespace Metric.Azdo.Api.Controllers;

/// <summary>
/// Controller for Azure DevOps DORA metrics
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoraController : ControllerBase
{
    private readonly AzureDevOpsService _azureDevOpsService;
    private readonly ILogger<DoraController> _logger;

    public DoraController(AzureDevOpsService azureDevOpsService, ILogger<DoraController> logger)
    {
        _azureDevOpsService = azureDevOpsService;
        _logger = logger;
    }

    /// <summary>
    /// Get DORA metrics for all projects
    /// </summary>
    /// <param name="periodDays">Period in days to calculate metrics for (default: 30)</param>
    /// <returns>List of DORA metrics for all projects</returns>
    [HttpGet]
    public async Task<ActionResult<List<DoraMetrics>>> GetAllProjectsMetrics([FromQuery] int periodDays = 30)
    {
        try
        {
            _logger.LogInformation("Getting DORA metrics for all projects (period: {PeriodDays} days)", periodDays);
            
            var metrics = await _azureDevOpsService.GetAllProjectsDoraMetricsAsync(periodDays);
            
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DORA metrics for all projects");
            return StatusCode(500, "Internal server error while retrieving metrics");
        }
    }

    /// <summary>
    /// Get DORA metrics for a specific project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="periodDays">Period in days to calculate metrics for (default: 30)</param>
    /// <returns>DORA metrics for the specified project</returns>
    [HttpGet("{projectId}")]
    public async Task<ActionResult<DoraMetrics>> GetProjectMetrics(string projectId, [FromQuery] int periodDays = 30)
    {
        try
        {
            _logger.LogInformation("Getting DORA metrics for project {ProjectId} (period: {PeriodDays} days)", projectId, periodDays);
            
            var metrics = await _azureDevOpsService.GetDoraMetricsAsync(projectId, periodDays);
            
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DORA metrics for project {ProjectId}", projectId);
            return StatusCode(500, "Internal server error while retrieving metrics");
        }
    }

    /// <summary>
    /// Get all Azure DevOps projects
    /// </summary>
    /// <returns>List of all projects in the organization</returns>
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            _logger.LogInformation("Getting all Azure DevOps projects");
            
            var projects = await _azureDevOpsService.GetProjectsAsync();
            
            var projectsInfo = projects.Select(p => new 
            { 
                Id = p.Id.ToString(), 
                Name = p.Name, 
                Description = p.Description,
                State = p.State.ToString(),
                Visibility = p.Visibility.ToString(),
                LastUpdateTime = p.LastUpdateTime
            }).ToList();
            
            return Ok(projectsInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Azure DevOps projects");
            return StatusCode(500, "Internal server error while retrieving projects");
        }
    }
}