# DORA Metrics Frontend

A beautiful and simple React TypeScript frontend for displaying DevOps Research and Assessment (DORA) metrics from Azure DevOps projects.

## Features

- 📊 **Project Selection**: Browse and select from available Azure DevOps projects
- 🎯 **DORA Metrics Dashboard**: View all four key DORA metrics:
  - Deployment Frequency
  - Lead Time for Changes
  - Change Failure Rate
  - Time to Recovery
- 📈 **Performance Levels**: Color-coded performance indicators (Elite, High, Medium, Low)
- 📱 **Responsive Design**: Works on desktop and mobile devices
- 🔄 **Real-time Data**: Connects directly to the API (no static data)
- ⏱️ **Configurable Time Periods**: View metrics for 7, 30, or 90 days

## Tech Stack

- **React 19** with TypeScript
- **Redux Toolkit** for state management
- **Axios** for API communication
- **Vite** for build tooling
- **CSS3** with modern styling

## Getting Started

### Prerequisites

- Node.js 16+ and npm
- The DORA Metrics API running (see [../Metric.Azdo.Api/README.md](../Metric.Azdo.Api/README.md))

### Installation

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

4. Open your browser to [http://localhost:5173](http://localhost:5173)

### Building for Production

```bash
npm run build
```

The built files will be in the `dist/` directory.

## API Integration

The frontend connects to the DORA Metrics API at `http://localhost:5000` by default. Make sure the API is running before starting the frontend.

### API Endpoints Used

- `GET /api/dora/projects` - Fetch available projects
- `GET /api/dora` - Fetch metrics for all projects
- `GET /api/dora/{projectId}` - Fetch metrics for a specific project

## Project Structure

```
src/
├── components/          # React components
│   ├── DoraDashboard.tsx    # Main dashboard
│   ├── MetricCard.tsx       # Individual metric display
│   └── ProjectSelector.tsx  # Project selection
├── hooks/              # Custom hooks
│   └── redux.ts            # Typed Redux hooks
├── services/           # API integration
│   └── api.ts              # API service layer
├── store/              # Redux store
│   ├── index.ts            # Store configuration
│   ├── metricsSlice.ts     # Metrics state
│   └── projectSlice.ts     # Projects state
├── types/              # TypeScript types
│   └── api.ts              # API response types
└── styles/             # CSS files
```

## Usage

1. **Select a Project**: Use the dropdown to select a specific project or view all projects
2. **Choose Time Period**: Select 7, 30, or 90 days to analyze different time ranges
3. **View Metrics**: Each metric card shows:
   - Current value with appropriate units
   - Performance level (Elite, High, Medium, Low)
   - Additional context information

## Performance Levels

The app displays performance levels based on industry DORA standards:

- 🟢 **Elite**: Top performers
- 🔵 **High**: High performers  
- 🟡 **Medium**: Medium performers
- 🔴 **Low**: Low performers

## Customization

### Changing API URL

Update the `API_BASE_URL` in `src/services/api.ts` to point to your API instance.

### Styling

The app uses CSS modules and can be customized by editing the `.css` files in the components directory.

## Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint

### Adding New Features

1. Follow the existing patterns for Redux slices
2. Use typed hooks from `src/hooks/redux.ts`
3. Add TypeScript types to `src/types/api.ts`
4. Maintain responsive design principles

## Troubleshooting

### API Connection Issues

- Ensure the backend API is running on port 5000
- Check CORS configuration in the API
- Verify network connectivity

### Build Issues

- Clear `node_modules` and reinstall: `rm -rf node_modules package-lock.json && npm install`
- Check TypeScript errors: `npm run build`

## Contributing

1. Follow the existing code style
2. Add TypeScript types for new features
3. Update this README for new functionality
4. Ensure responsive design works on mobile devices
