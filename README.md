# metric.azdo

A lightweight .NET service for retrieving DORA metrics from Azure DevOps.

## Features

This API provides simple HTTP endpoints to retrieve DevOps Research and Assessment (DORA) metrics from Azure DevOps projects:

- **Deployment Frequency** - How often deployments occur
- **Lead Time for Changes** - Time from code commit to deployment  
- **Change Failure Rate** - Percentage of deployments that cause failures
- **Time to Recovery** - Time to recover from failures

## API Endpoints

### GET /api/dora
Returns DORA metrics for all projects in the organization.

**Query Parameters:**
- `periodDays` (optional, default: 30) - Period in days to calculate metrics for

**Example Response:**
```json
[
  {
    "projectName": "MyProject",
    "projectId": "12345678-1234-1234-1234-123456789012",
    "deploymentFrequency": {
      "deploymentsCount": 15,
      "periodDays": 30,
      "deploymentsPerDay": 0.5,
      "performanceLevel": "High"
    },
    "leadTimeForChanges": {
      "averageLeadTimeHours": 4.2,
      "performanceLevel": "Elite"
    },
    "changeFailureRate": {
      "failedDeployments": 1,
      "totalDeployments": 15,
      "failureRatePercentage": 6.67,
      "performanceLevel": "High"
    },
    "timeToRecovery": {
      "averageRecoveryTimeHours": 0.5,
      "performanceLevel": "Elite"
    }
  }
]
```

### GET /api/dora/{projectId}
Returns DORA metrics for a specific project.

**Path Parameters:**
- `projectId` - The Azure DevOps project ID

**Query Parameters:**
- `periodDays` (optional, default: 30) - Period in days to calculate metrics for

### GET /api/dora/projects
Returns all Azure DevOps projects in the organization.

**Example Response:**
```json
[
  {
    "id": "12345678-1234-1234-1234-123456789012",
    "name": "MyProject",
    "description": "My project description",
    "state": "WellFormed",
    "visibility": "Private",
    "lastUpdateTime": "2024-01-15T10:30:00Z"
  }
]
```

## Configuration

Update `appsettings.json` with your Azure DevOps settings:

```json
{
  "AzureDevOps": {
    "OrganizationUrl": "https://dev.azure.com/your-organization",
    "PersonalAccessToken": "your-pat-token",
    "CollectionName": "DefaultCollection"
  }
}
```

### Personal Access Token

To create a Personal Access Token (PAT):

1. Go to your Azure DevOps organization
2. Click on your user profile → Personal Access Tokens
3. Create a new token with the following scopes:
   - **Build (Read)** - To read build information
   - **Project and Team (Read)** - To list projects
   - **Code (Read)** - To access source control information

## Performance Levels

The API categorizes DORA metrics into performance levels based on industry standards:

### Deployment Frequency
- **Elite**: Multiple times per day (≥ 1 deployment/day)
- **High**: Weekly (≥ 0.14 deployments/day)  
- **Medium**: Monthly (≥ 0.03 deployments/day)
- **Low**: Less than monthly

### Lead Time for Changes
- **Elite**: Less than 1 day (≤ 24 hours)
- **High**: Less than 1 week (≤ 168 hours)
- **Medium**: Less than 1 month (≤ 720 hours)
- **Low**: More than 1 month

### Change Failure Rate
- **Elite**: ≤ 5%
- **High**: ≤ 10%
- **Medium**: ≤ 15%
- **Low**: > 15%

### Time to Recovery
- **Elite**: Less than 1 hour (≤ 1 hour)
- **High**: Less than 1 day (≤ 24 hours)
- **Medium**: Less than 1 week (≤ 168 hours)
- **Low**: More than 1 week

## Running the Application

1. Clone the repository
2. Configure your Azure DevOps settings in `appsettings.json`
3. Run the application:

```bash
cd Metric.Azdo.Api
dotnet run
```

4. Open your browser to `http://localhost:5000/swagger` to view the API documentation

## Docker Support

Build and run with Docker:

```bash
# Build the image
docker build -t metric-azdo-api .

# Run the container
docker run -d -p 5000:80 -e AzureDevOps__OrganizationUrl="https://dev.azure.com/your-org" -e AzureDevOps__PersonalAccessToken="your-pat" metric-azdo-api
```

## Important Notes

- The service identifies deployment builds by looking for "deploy" or "release" in the build definition name
- Lead time calculation is simplified and uses build queue time to finish time (in production, you'd calculate from commit time)
- Metrics are calculated over a rolling period (default 30 days)
- The API handles authentication errors gracefully and returns appropriate HTTP status codes

## License

MIT License - see LICENSE file for details.
