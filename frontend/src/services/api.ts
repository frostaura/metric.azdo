import axios from 'axios';
import type { Project, DoraMetrics } from '../types/api';
import { mockProjects, mockMetrics, getMockProjectMetrics } from './mockData';

const API_BASE_URL = 'http://localhost:5000/api';
const USE_MOCK_DATA = false; // Set to true for demo purposes

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const apiService = {
  // Get all projects
  async getProjects(): Promise<Project[]> {
    if (USE_MOCK_DATA) {
      return mockProjects;
    }
    
    try {
      const response = await apiClient.get<Project[]>('/dora/projects');
      return response.data;
    } catch (error) {
      console.warn('API failed, falling back to mock data:', error);
      return mockProjects;
    }
  },

  // Get DORA metrics for all projects
  async getAllProjectsMetrics(periodDays: number = 30): Promise<DoraMetrics[]> {
    if (USE_MOCK_DATA) {
      return mockMetrics;
    }
    
    try {
      const response = await apiClient.get<DoraMetrics[]>(`/dora?periodDays=${periodDays}`);
      return response.data;
    } catch (error) {
      console.warn('API failed, falling back to mock data:', error);
      return mockMetrics;
    }
  },

  // Get DORA metrics for a specific project
  async getProjectMetrics(projectId: string, periodDays: number = 30): Promise<DoraMetrics> {
    if (USE_MOCK_DATA) {
      return getMockProjectMetrics(projectId);
    }
    
    try {
      const response = await apiClient.get<DoraMetrics>(`/dora/${projectId}?periodDays=${periodDays}`);
      return response.data;
    } catch (error) {
      console.warn('API failed, falling back to mock data:', error);
      return getMockProjectMetrics(projectId);
    }
  },
};

export default apiService;