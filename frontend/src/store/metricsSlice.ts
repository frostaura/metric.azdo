import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { DoraMetrics } from '../types/api';
import { apiService } from '../services/api';

export interface MetricsState {
  allProjectsMetrics: DoraMetrics[];
  selectedProjectMetrics: DoraMetrics | null;
  periodDays: number;
  loading: boolean;
  error: string | null;
}

const initialState: MetricsState = {
  allProjectsMetrics: [],
  selectedProjectMetrics: null,
  periodDays: 30,
  loading: false,
  error: null,
};

// Async thunk for fetching all projects metrics
export const fetchAllProjectsMetrics = createAsyncThunk(
  'metrics/fetchAllProjectsMetrics',
  async (periodDays: number = 30) => {
    const metrics = await apiService.getAllProjectsMetrics(periodDays);
    return metrics;
  }
);

// Async thunk for fetching specific project metrics
export const fetchProjectMetrics = createAsyncThunk(
  'metrics/fetchProjectMetrics',
  async (params: { projectId: string; periodDays: number }) => {
    const metrics = await apiService.getProjectMetrics(params.projectId, params.periodDays);
    return metrics;
  }
);

const metricsSlice = createSlice({
  name: 'metrics',
  initialState,
  reducers: {
    setPeriodDays: (state, action: PayloadAction<number>) => {
      state.periodDays = action.payload;
    },
    clearSelectedMetrics: (state) => {
      state.selectedProjectMetrics = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // All projects metrics
      .addCase(fetchAllProjectsMetrics.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllProjectsMetrics.fulfilled, (state, action) => {
        state.loading = false;
        state.allProjectsMetrics = action.payload;
      })
      .addCase(fetchAllProjectsMetrics.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch metrics';
      })
      // Project specific metrics
      .addCase(fetchProjectMetrics.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProjectMetrics.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedProjectMetrics = action.payload;
      })
      .addCase(fetchProjectMetrics.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch project metrics';
      });
  },
});

export const { setPeriodDays, clearSelectedMetrics, clearError } = metricsSlice.actions;
export default metricsSlice.reducer;