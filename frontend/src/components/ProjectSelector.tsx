import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../hooks/redux';
import { fetchProjects, setSelectedProject } from '../store/projectSlice';
import './ProjectSelector.css';

const ProjectSelector: React.FC = () => {
  const dispatch = useAppDispatch();
  const { projects, selectedProjectId, loading, error } = useAppSelector(state => state.projects);

  useEffect(() => {
    dispatch(fetchProjects());
  }, [dispatch]);

  const handleProjectChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const projectId = event.target.value || null;
    dispatch(setSelectedProject(projectId));
  };

  if (loading) {
    return (
      <div className="project-selector loading">
        <div className="loading-spinner"></div>
        <p>Loading projects...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="project-selector error">
        <p>Error loading projects: {error}</p>
        <button onClick={() => dispatch(fetchProjects())}>Retry</button>
      </div>
    );
  }

  return (
    <div className="project-selector">
      <h2>Select Project</h2>
      <div className="select-wrapper">
        <select 
          value={selectedProjectId || ''} 
          onChange={handleProjectChange}
          className="project-select"
        >
          <option value="">Select a project...</option>
          {projects.map(project => (
            <option key={project.id} value={project.id}>
              {project.name}
            </option>
          ))}
        </select>
      </div>
      {projects.length === 0 && !loading && (
        <p className="no-projects">No projects available</p>
      )}
    </div>
  );
};

export default ProjectSelector;