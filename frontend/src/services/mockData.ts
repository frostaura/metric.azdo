import type { Project, DoraMetrics } from '../types/api';

// Mock data for demo purposes when API is not available or configured
export const mockProjects: Project[] = [
  {
    id: "mock-project-1",
    name: "E-Commerce Platform",
    description: "Main e-commerce platform with microservices architecture",
    state: "WellFormed",
    visibility: "Private",
    lastUpdateTime: "2024-01-15T10:30:00Z"
  },
  {
    id: "mock-project-2", 
    name: "Mobile App Backend",
    description: "Backend services for mobile applications",
    state: "WellFormed",
    visibility: "Private",
    lastUpdateTime: "2024-01-14T14:20:00Z"
  },
  {
    id: "mock-project-3",
    name: "Analytics Dashboard",
    description: "Business intelligence and analytics platform",
    state: "WellFormed", 
    visibility: "Private",
    lastUpdateTime: "2024-01-13T09:15:00Z"
  }
];

export const mockMetrics: DoraMetrics[] = [
  {
    projectName: "E-Commerce Platform",
    projectId: "mock-project-1",
    deploymentFrequency: {
      deploymentsCount: 42,
      periodDays: 30,
      deploymentsPerDay: 1.4,
      performanceLevel: "Elite"
    },
    leadTimeForChanges: {
      averageLeadTimeHours: 18.5,
      performanceLevel: "Elite"
    },
    changeFailureRate: {
      failedDeployments: 2,
      totalDeployments: 42,
      failureRatePercentage: 4.76,
      performanceLevel: "Elite"
    },
    timeToRecovery: {
      averageRecoveryTimeHours: 0.8,
      performanceLevel: "Elite"
    }
  },
  {
    projectName: "Mobile App Backend",
    projectId: "mock-project-2",
    deploymentFrequency: {
      deploymentsCount: 12,
      periodDays: 30,
      deploymentsPerDay: 0.4,
      performanceLevel: "High"
    },
    leadTimeForChanges: {
      averageLeadTimeHours: 68.2,
      performanceLevel: "High"
    },
    changeFailureRate: {
      failedDeployments: 1,
      totalDeployments: 12,
      failureRatePercentage: 8.33,
      performanceLevel: "High"
    },
    timeToRecovery: {
      averageRecoveryTimeHours: 12.5,
      performanceLevel: "High"
    }
  },
  {
    projectName: "Analytics Dashboard",
    projectId: "mock-project-3",
    deploymentFrequency: {
      deploymentsCount: 4,
      periodDays: 30,
      deploymentsPerDay: 0.133,
      performanceLevel: "Medium"
    },
    leadTimeForChanges: {
      averageLeadTimeHours: 264.8,
      performanceLevel: "Medium"
    },
    changeFailureRate: {
      failedDeployments: 1,
      totalDeployments: 4,
      failureRatePercentage: 25.0,
      performanceLevel: "Low"
    },
    timeToRecovery: {
      averageRecoveryTimeHours: 48.2,
      performanceLevel: "Medium"
    }
  }
];

export const getMockProjectMetrics = (projectId: string): DoraMetrics => {
  return mockMetrics.find(m => m.projectId === projectId) || mockMetrics[0];
};