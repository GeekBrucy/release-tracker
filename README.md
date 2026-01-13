# Release Tracker

A simple internal application for tracking application releases, designed for both technical and non-technical users.

## Overview

Release Tracker allows organizations to:
- Create and manage applications (containers for releases)
- Track release records for each application
- View release history and details
- Filter and search releases by various criteria

## Technology Stack

- **Framework**: ASP.NET Core MVC 9.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core 9.0
- **UI**: Bootstrap 5
- **Deployment**: IIS Server (internal network)

## Prerequisites

- .NET 9.0 SDK
- SQL Server (Express, Developer, or Enterprise)
- Visual Studio 2022 or VS Code with C# Dev Kit
- SQL Server Management Studio (optional)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd release-tracker
```

### 2. Configure Database Connection

Update the connection string in `ReleaseTracker.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ReleaseTrackerDev;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

### 3. Apply Database Migrations

```bash
cd ReleaseTracker.Web
dotnet ef database update
```

This will create the database and all required tables.

### 4. Run the Application

```bash
dotnet run
```

Navigate to `https://localhost:5001` (or the URL shown in the console).

## Project Structure

```
release-tracker/
├── ReleaseTracker.Web/              # Main MVC application
│   ├── Controllers/                 # MVC Controllers
│   ├── Data/                        # DbContext
│   ├── Migrations/                  # EF Core migrations
│   ├── Models/                      # Entity models
│   ├── Views/                       # Razor views
│   ├── wwwroot/                     # Static files
│   ├── appsettings.json            # Configuration
│   └── Program.cs                   # Application entry point
├── ReleaseTracker.sln               # Solution file
└── README.md                        # This file
```

## Database Schema

### Apps Table
- **Id**: Primary key
- **Name**: Application name (unique, required)
- **Description**: Application description
- **CreatedDate**: When the app was created
- **CreatedBy**: Who created the app
- **IsActive**: Soft delete flag

### Releases Table
- **Id**: Primary key
- **AppId**: Foreign key to Apps table
- **Version**: Release version number
- **ReleaseDate**: When the release occurred
- **ReleasedBy**: Who released it
- **Description**: Release description
- **ReleaseNotes**: Detailed release notes
- **Environment**: Deployment environment (Dev/Test/Staging/Production)
- **Status**: Release status (Planned/In Progress/Completed/Rolled Back)
- **CreatedDate**: When the record was created
- **CreatedBy**: Who created the record
- **ModifiedDate**: Last modification date
- **ModifiedBy**: Who last modified the record

## Features

### Configurable Dropdown Options
One of the key features is **fully configurable dropdown menus** for Environment and Status fields:

To customize dropdown options, simply edit `appsettings.json`:

```json
{
  "ReleaseOptions": {
    "Environments": [
      "Development",
      "Test",
      "Staging",
      "Production"
    ],
    "Statuses": [
      "Planned",
      "In Progress",
      "Completed",
      "Rolled Back"
    ]
  }
}
```

**No code changes required!** Just restart the application after editing the configuration.

### Applications Management
- Create and manage application containers
- Search and filter applications
- Soft delete (deactivate) applications
- View application details with recent releases

### Release Tracking
- Create releases for applications
- Track version, environment, and status
- Add detailed release notes
- Filter releases by application, environment, or status
- Audit trail (created/modified dates and users)

### Dashboard
- Summary statistics (total apps, total releases, releases this week)
- Quick action buttons
- Recent releases table with color-coded status badges

## Development Status

### ✅ Stage 1: Project Setup & Database Foundation (COMPLETED)
- ASP.NET Core MVC project created
- Entity Framework Core configured
- Database schema designed and migrated
- Entity models with validation created

### ✅ Stage 2-4: Complete UI Implementation (COMPLETED)
- Apps CRUD operations with search/filter
- Releases CRUD operations with configurable dropdowns
- Soft delete for applications
- Dashboard with statistics and recent releases
- Bootstrap 5 responsive design
- User-friendly interface for non-technical users
- Audit tracking for all changes

### 📋 Stage 5: IIS Deployment (PLANNED)
- Production configuration
- IIS setup and deployment
- Post-deployment verification

## Commands Reference

### Entity Framework

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script from migrations
dotnet ef migrations script
```

### Build & Run

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Publish for IIS
dotnet publish -c Release -o ./publish
```

## Contributing

This is an internal project. For questions or issues, please contact the development team.

## License

Internal use only.
