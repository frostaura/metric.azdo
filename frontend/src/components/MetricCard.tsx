import React from 'react';
import type { PerformanceLevel } from '../types/api';
import './MetricCard.css';

interface MetricCardProps {
  title: string;
  value: string;
  description: string;
  performanceLevel: PerformanceLevel;
  icon?: string;
}

const MetricCard: React.FC<MetricCardProps> = ({
  title,
  value,
  description,
  performanceLevel,
  icon = '📊'
}) => {
  const getPerformanceLevelColor = (level: PerformanceLevel): string => {
    switch (level) {
      case 'Elite':
        return '#10b981'; // Green
      case 'High':
        return '#3b82f6'; // Blue
      case 'Medium':
        return '#f59e0b'; // Amber
      case 'Low':
        return '#ef4444'; // Red
      default:
        return '#6b7280'; // Gray
    }
  };

  return (
    <div 
      className="metric-card"
      data-performance={performanceLevel.toLowerCase()}
      style={{ '--performance-color': getPerformanceLevelColor(performanceLevel) } as React.CSSProperties}
    >
      <div className="metric-header">
        <div className="metric-icon">{icon}</div>
        <div className="metric-info">
          <h3 className="metric-title">{title}</h3>
          <div className="metric-level" data-level={performanceLevel.toLowerCase()}>
            {performanceLevel}
          </div>
        </div>
      </div>
      <div className="metric-value">{value}</div>
      <p className="metric-description">{description}</p>
    </div>
  );
};

export default MetricCard;