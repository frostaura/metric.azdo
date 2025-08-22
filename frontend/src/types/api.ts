export interface Project {
  id: string;
  name: string;
  description: string;
  state: string;
  visibility: string;
  lastUpdateTime: string;
}

export interface DeploymentFrequency {
  deploymentsCount: number;
  periodDays: number;
  deploymentsPerDay: number;
  performanceLevel: string;
}

export interface LeadTimeForChanges {
  averageLeadTimeHours: number;
  performanceLevel: string;
}

export interface ChangeFailureRate {
  failedDeployments: number;
  totalDeployments: number;
  failureRatePercentage: number;
  performanceLevel: string;
}

export interface TimeToRecovery {
  averageRecoveryTimeHours: number;
  performanceLevel: string;
}

export interface DoraMetrics {
  projectName: string;
  projectId: string;
  deploymentFrequency: DeploymentFrequency;
  leadTimeForChanges: LeadTimeForChanges;
  changeFailureRate: ChangeFailureRate;
  timeToRecovery: TimeToRecovery;
}

export type PerformanceLevel = 'Elite' | 'High' | 'Medium' | 'Low';