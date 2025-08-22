import ProjectSelector from './components/ProjectSelector';
import DoraDashboard from './components/DoraDashboard';
import './App.css';

function App() {
  return (
    <div className="app">
      <main className="main-content">
        <ProjectSelector />
        <DoraDashboard />
      </main>
    </div>
  );
}

export default App
