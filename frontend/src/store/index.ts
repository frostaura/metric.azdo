import { configureStore } from '@reduxjs/toolkit';
import projectReducer from './projectSlice';
import metricsReducer from './metricsSlice';

export const store = configureStore({
  reducer: {
    projects: projectReducer,
    metrics: metricsReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;