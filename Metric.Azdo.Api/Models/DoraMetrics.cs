namespace Metric.Azdo.Api.Models;

/// <summary>
/// DORA metrics for a project
/// </summary>
public class DoraMetrics
{
    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Project ID
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Deployment frequency - How often deployments occur
    /// </summary>
    public DeploymentFrequency DeploymentFrequency { get; set; } = new();

    /// <summary>
    /// Lead time for changes - Time from code commit to deployment
    /// </summary>
    public LeadTimeForChanges LeadTimeForChanges { get; set; } = new();

    /// <summary>
    /// Change failure rate - Percentage of deployments that cause failures
    /// </summary>
    public ChangeFailureRate ChangeFailureRate { get; set; } = new();

    /// <summary>
    /// Time to recovery - Time to recover from failures
    /// </summary>
    public TimeToRecovery TimeToRecovery { get; set; } = new();
}

/// <summary>
/// Deployment frequency metric
/// </summary>
public class DeploymentFrequency
{
    /// <summary>
    /// Number of deployments in the period
    /// </summary>
    public int DeploymentsCount { get; set; }

    /// <summary>
    /// Period in days
    /// </summary>
    public int PeriodDays { get; set; } = 30;

    /// <summary>
    /// Deployments per day
    /// </summary>
    public double DeploymentsPerDay => PeriodDays > 0 ? (double)DeploymentsCount / PeriodDays : 0;

    /// <summary>
    /// Performance level (Elite, High, Medium, Low)
    /// </summary>
    public string PerformanceLevel => DeploymentsPerDay switch
    {
        >= 1 => "Elite",
        >= 0.14 => "High", // Weekly
        >= 0.03 => "Medium", // Monthly
        _ => "Low"
    };
}

/// <summary>
/// Lead time for changes metric
/// </summary>
public class LeadTimeForChanges
{
    /// <summary>
    /// Average lead time in hours
    /// </summary>
    public double AverageLeadTimeHours { get; set; }

    /// <summary>
    /// Performance level (Elite, High, Medium, Low)
    /// </summary>
    public string PerformanceLevel => AverageLeadTimeHours switch
    {
        <= 24 => "Elite", // Less than 1 day
        <= 168 => "High", // Less than 1 week
        <= 720 => "Medium", // Less than 1 month
        _ => "Low"
    };
}

/// <summary>
/// Change failure rate metric
/// </summary>
public class ChangeFailureRate
{
    /// <summary>
    /// Number of failed deployments
    /// </summary>
    public int FailedDeployments { get; set; }

    /// <summary>
    /// Total number of deployments
    /// </summary>
    public int TotalDeployments { get; set; }

    /// <summary>
    /// Failure rate as percentage
    /// </summary>
    public double FailureRatePercentage => TotalDeployments > 0 ? (double)FailedDeployments / TotalDeployments * 100 : 0;

    /// <summary>
    /// Performance level (Elite, High, Medium, Low)
    /// </summary>
    public string PerformanceLevel => FailureRatePercentage switch
    {
        <= 5 => "Elite",
        <= 10 => "High",
        <= 15 => "Medium",
        _ => "Low"
    };
}

/// <summary>
/// Time to recovery metric
/// </summary>
public class TimeToRecovery
{
    /// <summary>
    /// Average recovery time in hours
    /// </summary>
    public double AverageRecoveryTimeHours { get; set; }

    /// <summary>
    /// Performance level (Elite, High, Medium, Low)
    /// </summary>
    public string PerformanceLevel => AverageRecoveryTimeHours switch
    {
        <= 1 => "Elite", // Less than 1 hour
        <= 24 => "High", // Less than 1 day
        <= 168 => "Medium", // Less than 1 week
        _ => "Low"
    };
}