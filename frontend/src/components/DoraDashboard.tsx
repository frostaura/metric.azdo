import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../hooks/redux';
import { fetchProjectMetrics, fetchAllProjectsMetrics, setPeriodDays } from '../store/metricsSlice';
import MetricCard from './MetricCard';
import './DoraDashboard.css';

const DoraDashboard: React.FC = () => {
  const dispatch = useAppDispatch();
  const { selectedProjectId } = useAppSelector(state => state.projects);
  const { 
    selectedProjectMetrics, 
    allProjectsMetrics, 
    periodDays, 
    loading, 
    error 
  } = useAppSelector(state => state.metrics);

  // Fetch metrics when project selection or period changes
  useEffect(() => {
    if (selectedProjectId) {
      dispatch(fetchProjectMetrics({ projectId: selectedProjectId, periodDays }));
    } else {
      dispatch(fetchAllProjectsMetrics(periodDays));
    }
  }, [selectedProjectId, periodDays, dispatch]);

  const handlePeriodChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    dispatch(setPeriodDays(Number(event.target.value)));
  };

  const formatValue = (value: number, unit: string): string => {
    if (value === 0) return `0 ${unit}`;
    if (value < 1) return `${value.toFixed(2)} ${unit}`;
    return `${value.toFixed(1)} ${unit}`;
  };

  if (loading) {
    return (
      <div className="dashboard loading">
        <div className="loading-spinner"></div>
        <p>Loading DORA metrics...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard error">
        <h2>Error Loading Metrics</h2>
        <p>{error}</p>
        <button 
          onClick={() => {
            if (selectedProjectId) {
              dispatch(fetchProjectMetrics({ projectId: selectedProjectId, periodDays }));
            } else {
              dispatch(fetchAllProjectsMetrics(periodDays));
            }
          }}
        >
          Retry
        </button>
      </div>
    );
  }

  if (!selectedProjectId && allProjectsMetrics.length === 0) {
    return (
      <div className="dashboard empty">
        <h2>No Data Available</h2>
        <p>Please select a project or ensure projects have deployment data available.</p>
      </div>
    );
  }

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <div className="title-section">
          <h1>DORA Metrics Dashboard</h1>
          {selectedProjectId && selectedProjectMetrics && (
            <p className="project-name">Project: {selectedProjectMetrics.projectName}</p>
          )}
        </div>
        
        <div className="controls">
          <label htmlFor="period-select">Time Period:</label>
          <select 
            id="period-select"
            value={periodDays} 
            onChange={handlePeriodChange}
            className="period-select"
          >
            <option value={7}>Last 7 days</option>
            <option value={30}>Last 30 days</option>
            <option value={90}>Last 90 days</option>
          </select>
        </div>
      </div>

      {selectedProjectId && selectedProjectMetrics ? (
        // Single project view
        <div className="metrics-grid">
          <MetricCard
            title="Deployment Frequency"
            value={formatValue(selectedProjectMetrics.deploymentFrequency.deploymentsPerDay, "per day")}
            description={`${selectedProjectMetrics.deploymentFrequency.deploymentsCount} deployments in ${periodDays} days`}
            performanceLevel={selectedProjectMetrics.deploymentFrequency.performanceLevel as any}
            icon="🚀"
          />
          
          <MetricCard
            title="Lead Time for Changes"
            value={formatValue(selectedProjectMetrics.leadTimeForChanges.averageLeadTimeHours, "hours")}
            description="Average time from code commit to deployment"
            performanceLevel={selectedProjectMetrics.leadTimeForChanges.performanceLevel as any}
            icon="⏱️"
          />
          
          <MetricCard
            title="Change Failure Rate"
            value={`${selectedProjectMetrics.changeFailureRate.failureRatePercentage.toFixed(1)}%`}
            description={`${selectedProjectMetrics.changeFailureRate.failedDeployments} of ${selectedProjectMetrics.changeFailureRate.totalDeployments} deployments failed`}
            performanceLevel={selectedProjectMetrics.changeFailureRate.performanceLevel as any}
            icon="🛠️"
          />
          
          <MetricCard
            title="Time to Recovery"
            value={formatValue(selectedProjectMetrics.timeToRecovery.averageRecoveryTimeHours, "hours")}
            description="Average time to recover from deployment failures"
            performanceLevel={selectedProjectMetrics.timeToRecovery.performanceLevel as any}
            icon="🔧"
          />
        </div>
      ) : (
        // All projects view
        <div className="projects-overview">
          <h2>All Projects Overview</h2>
          {allProjectsMetrics.map((metrics) => (
            <div key={metrics.projectId} className="project-summary">
              <h3>{metrics.projectName}</h3>
              <div className="metrics-grid">
                <MetricCard
                  title="Deployment Frequency"
                  value={formatValue(metrics.deploymentFrequency.deploymentsPerDay, "per day")}
                  description={`${metrics.deploymentFrequency.deploymentsCount} deployments`}
                  performanceLevel={metrics.deploymentFrequency.performanceLevel as any}
                  icon="🚀"
                />
                
                <MetricCard
                  title="Lead Time"
                  value={formatValue(metrics.leadTimeForChanges.averageLeadTimeHours, "hours")}
                  description="Avg. lead time"
                  performanceLevel={metrics.leadTimeForChanges.performanceLevel as any}
                  icon="⏱️"
                />
                
                <MetricCard
                  title="Failure Rate"
                  value={`${metrics.changeFailureRate.failureRatePercentage.toFixed(1)}%`}
                  description="Change failures"
                  performanceLevel={metrics.changeFailureRate.performanceLevel as any}
                  icon="🛠️"
                />
                
                <MetricCard
                  title="Recovery Time"
                  value={formatValue(metrics.timeToRecovery.averageRecoveryTimeHours, "hours")}
                  description="Avg. recovery"
                  performanceLevel={metrics.timeToRecovery.performanceLevel as any}
                  icon="🔧"
                />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default DoraDashboard;