# Release Tracker - Development Log

## Session: 2026-01-13

### Stage 1: Project Setup & Database Foundation

#### 1. Project Creation
**Time**: Initial setup
**Status**: ✅ Completed

**Actions**:
- Created ASP.NET Core MVC project (.NET 9.0): `ReleaseTracker.Web`
- Created solution file: `ReleaseTracker.sln`
- Added project to solution

**Commands**:
```bash
dotnet new mvc -n ReleaseTracker.Web -o ReleaseTracker.Web
dotnet new sln -n ReleaseTracker
dotnet sln ReleaseTracker.sln add ReleaseTracker.Web/ReleaseTracker.Web.csproj
```

**Result**: Solution structure created successfully

---

#### 2. NuGet Packages Installation
**Time**: Following project creation
**Status**: ✅ Completed

**Actions**:
- Installed Entity Framework Core 9.0 packages (compatible with .NET 9.0)
  - `Microsoft.EntityFrameworkCore.SqlServer` (9.0.0)
  - `Microsoft.EntityFrameworkCore.Tools` (9.0.0)
  - `Microsoft.EntityFrameworkCore.Design` (9.0.0)
  - `Microsoft.VisualStudio.Web.CodeGeneration.Design` (9.0.0)

**Commands**:
```bash
cd ReleaseTracker.Web
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 9.0.0
```

**Note**: Initially attempted to install EF Core 10.0 but encountered compatibility issue with .NET 9.0. Resolved by using EF Core 9.0.

**Result**: All packages restored successfully

---

#### 3. Entity Classes Creation
**Time**: Following package installation
**Status**: ✅ Completed

**Actions**:
- Created `Models/App.cs` entity with:
  - Properties: Id, Name, Description, CreatedDate, CreatedBy, IsActive
  - Data annotations for validation and display
  - Navigation property to Releases collection

- Created `Models/Release.cs` entity with:
  - Properties: Id, AppId, Version, ReleaseDate, ReleasedBy, Description, ReleaseNotes, Environment, Status, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
  - Data annotations for validation and display
  - Foreign key relationship to App entity

**Result**: Entity models created with proper validation and relationships

---

#### 4. DbContext Creation
**Time**: After entity classes
**Status**: ✅ Completed

**Actions**:
- Created `Data/ReleaseTrackerContext.cs`
- Configured entity relationships:
  - One-to-many relationship between Apps and Releases
  - Unique index on App.Name
  - Indexes on Release.AppId and Release.ReleaseDate (descending)
  - Prevent cascade delete (Restrict behavior)
- Set default values for CreatedDate and IsActive using SQL

**Result**: DbContext configured with proper entity configurations

---

#### 5. Connection String Configuration
**Time**: After DbContext
**Status**: ✅ Completed

**Actions**:
- Updated `appsettings.json` with development connection string
- Created `appsettings.Production.json` with production placeholder
- Registered DbContext in `Program.cs` with dependency injection
- Used SQL Server with SQL Authentication (localhost:1433)

**Connection String**:
```
Server=localhost,1433;Database=ReleaseTrackerDev;User Id=sa;Password=Password1;TrustServerCertificate=True
```

**Result**: Configuration files updated and DbContext registered successfully

---

#### 6. Initial Migration
**Time**: After configuration
**Status**: ✅ Completed

**Actions**:
- Created initial migration: `20260113043201_InitialCreate`
- Migration files generated in Migrations folder
- Applied migration to database using `dotnet ef database update`

**Migration Created**:
- Apps table with unique index on Name
- Releases table with foreign key to Apps
- Indexes on AppId and ReleaseDate (descending)
- Default values for CreatedDate and IsActive

**Result**: Database `ReleaseTrackerDev` created successfully with both tables and all indexes

---

#### 7. Verification
**Time**: Final step
**Status**: ✅ Completed

**Actions**:
- Ran `dotnet build` - Build succeeded with 0 errors, 0 warnings
- Verified migration files exist
- Verified database tables created:
  - Apps table with UK_Apps_Name unique index
  - Releases table with IX_Releases_AppId and IX_Releases_ReleaseDate indexes
  - Foreign key constraint FK_Releases_Apps_AppId

**Result**: Stage 1 complete - project compiles and database is ready

---

## Notes & Decisions

### Technology Stack Confirmed
- **Framework**: ASP.NET Core MVC 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server (localhost)
- **UI Framework**: Bootstrap 5 (included with template)

### Database Configuration
- **Development Database**: ReleaseTrackerDev
- **Authentication**: SQL Authentication (sa user)
- **Connection**: localhost:1433

### Project Structure
```
release-tracker/
├── ReleaseTracker.Web/
│   ├── Controllers/              # (default controllers)
│   ├── Data/
│   │   └── ReleaseTrackerContext.cs
│   ├── Migrations/
│   │   ├── 20260113043201_InitialCreate.cs
│   │   ├── 20260113043201_InitialCreate.Designer.cs
│   │   └── ReleaseTrackerContextModelSnapshot.cs
│   ├── Models/
│   │   ├── App.cs
│   │   ├── Release.cs
│   │   └── ErrorViewModel.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   └── Program.cs
├── ReleaseTracker.sln
└── DEVELOPMENT_LOG.md
```

---

## Stage 1 Summary ✅

**Goal**: Working .NET MVC project with database connection
**Status**: COMPLETED

**Success Criteria Met**:
- ✅ Solution compiles without errors
- ✅ Database created with Apps and Releases tables
- ✅ Migrations run successfully
- ✅ Can query database from application

---

---

## Session: 2026-01-13 (Continued) - Stages 2-4: UI Implementation

### Stage 2-4 Combined: Complete UI Build

#### 1. Configurable Dropdown Options
**Time**: Start of UI development
**Status**: ✅ Completed

**Actions**:
- Created `Models/ReleaseOptions.cs` for configurable dropdown values
- Added configuration section in `appsettings.json`:
  - Environments: Development, Test, Staging, Production
  - Statuses: Planned, In Progress, Completed, Rolled Back
- Registered ReleaseOptions in `Program.cs` using IOptions pattern

**Result**: Environment and Status dropdowns are now fully configurable via appsettings.json

---

#### 2. Apps Controller & Views
**Time**: After configuration setup
**Status**: ✅ Completed

**Actions**:
- Created `Controllers/AppsController.cs` with complete CRUD operations
- Implemented soft delete (IsActive flag)
- Auto-populate audit fields (CreatedDate, CreatedBy)
- Search/filter functionality in Index view

**Views Created**:
- `Views/Apps/Index.cshtml` - List all active apps with search
- `Views/Apps/Create.cshtml` - Create new application
- `Views/Apps/Edit.cshtml` - Edit existing application
- `Views/Apps/Details.cshtml` - View app details with recent releases
- `Views/Apps/Delete.cshtml` - Soft delete confirmation

**Features**:
- Bootstrap 5 styling with responsive design
- Success/error messages using TempData
- Intuitive forms with placeholders and help text
- Required field indicators (*)
- Breadcrumb navigation
- Action buttons with icons

**Result**: Complete Apps CRUD functionality with user-friendly interface

---

#### 3. Releases Controller & Views
**Time**: After Apps implementation
**Status**: ✅ Completed

**Actions**:
- Created `Controllers/ReleasesController.cs` with complete CRUD operations
- Integrated IOptions<ReleaseOptions> for configurable dropdowns
- Multi-filter support (by app, environment, status)
- Auto-populate audit tracking (CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)

**Views Created**:
- `Views/Releases/Index.cshtml` - List releases with advanced filtering
- `Views/Releases/Create.cshtml` - Create release with configurable dropdowns
- `Views/Releases/Edit.cshtml` - Edit release with audit tracking
- `Views/Releases/Details.cshtml` - View complete release information
- `Views/Releases/Delete.cshtml` - Delete confirmation with warning

**Key Features**:
- **Configurable dropdowns** for Environment and Status (from appsettings.json)
- Color-coded status badges (Success, Primary, Info, Danger)
- DateTime picker for release date
- Large text areas for release notes
- Filter by multiple criteria simultaneously
- Link to parent application from release view

**Result**: Complete Releases CRUD with configurable dropdowns as requested

---

#### 4. Dashboard (Home Controller)
**Time**: After CRUD completion
**Status**: ✅ Completed

**Actions**:
- Updated `Controllers/HomeController.cs` with dashboard statistics
- Injected ReleaseTrackerContext for data access

**Dashboard Features**:
- **Summary Cards**:
  - Total Applications (with count)
  - Total Releases (with count)
  - Releases This Week (with date range)
- **Quick Actions**:
  - Create New Application button
  - Create New Release button
- **Recent Releases Table**:
  - Last 10 releases
  - Sorted by release date (descending)
  - Color-coded status badges
  - Links to app and release details

**Result**: Informative dashboard providing quick overview and access

---

#### 5. Layout & Navigation
**Time**: Final UI polish
**Status**: ✅ Completed

**Actions**:
- Updated `Views/Shared/_Layout.cshtml`:
  - Changed to dark navbar for better contrast
  - Added Bootstrap Icons CDN
  - Updated navigation menu:
    - Dashboard (Home)
    - Applications
    - Releases
  - Removed Privacy link (not needed)
  - Updated footer with dynamic year
  - Changed container to container-fluid for full width

**Result**: Professional, intuitive navigation with clear visual hierarchy

---

## Stage 2-4 Summary ✅

**Goal**: Build complete user interface with configurable options
**Status**: COMPLETED

**Success Criteria Met**:
- ✅ Apps CRUD operations functional
- ✅ Releases CRUD operations functional
- ✅ Environment dropdown configurable via appsettings.json
- ✅ Status dropdown configurable via appsettings.json
- ✅ Search and filter functionality implemented
- ✅ Soft delete for apps implemented
- ✅ Audit tracking (Created/Modified fields) working
- ✅ Dashboard with statistics and recent releases
- ✅ User-friendly interface for non-technical users
- ✅ Responsive Bootstrap 5 design
- ✅ Success/error message feedback
- ✅ Application builds and runs successfully (http://localhost:5000)

---

## Technical Highlights

### Configurable Dropdowns Implementation
The Environment and Status fields are now fully configurable:

**Configuration** (appsettings.json):
```json
{
  "ReleaseOptions": {
    "Environments": ["Development", "Test", "Staging", "Production"],
    "Statuses": ["Planned", "In Progress", "Completed", "Rolled Back"]
  }
}
```

**To modify dropdown options**: Simply edit the arrays in appsettings.json - no code changes required!

**Usage in Controller**:
```csharp
public ReleasesController(ReleaseTrackerContext context, IOptions<ReleaseOptions> releaseOptions)
{
    _releaseOptions = releaseOptions.Value;
}

ViewBag.Environments = new SelectList(_releaseOptions.Environments);
ViewBag.Statuses = new SelectList(_releaseOptions.Statuses);
```

---

## Project Structure (Updated)

```
release-tracker/
├── ReleaseTracker.Web/
│   ├── Controllers/
│   │   ├── HomeController.cs           ✅ Updated with dashboard stats
│   │   ├── AppsController.cs           ✅ NEW - Complete CRUD
│   │   └── ReleasesController.cs       ✅ NEW - Complete CRUD with config
│   ├── Data/
│   │   └── ReleaseTrackerContext.cs
│   ├── Migrations/
│   │   └── 20260113043201_InitialCreate.cs
│   ├── Models/
│   │   ├── App.cs
│   │   ├── Release.cs
│   │   ├── ReleaseOptions.cs           ✅ NEW - Configuration model
│   │   └── ErrorViewModel.cs
│   ├── Views/
│   │   ├── Shared/
│   │   │   └── _Layout.cshtml          ✅ Updated navigation
│   │   ├── Home/
│   │   │   └── Index.cshtml            ✅ Updated dashboard
│   │   ├── Apps/                       ✅ NEW - All views
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   ├── Edit.cshtml
│   │   │   ├── Details.cshtml
│   │   │   └── Delete.cshtml
│   │   └── Releases/                   ✅ NEW - All views
│   │       ├── Index.cshtml
│   │       ├── Create.cshtml
│   │       ├── Edit.cshtml
│   │       ├── Details.cshtml
│   │       └── Delete.cshtml
│   ├── appsettings.json                ✅ Updated with ReleaseOptions
│   ├── appsettings.Production.json
│   └── Program.cs                      ✅ Updated with IOptions
├── ReleaseTracker.sln
├── DEVELOPMENT_LOG.md
├── README.md
└── .gitignore
```

---

## Next Session Tasks - Stage 5: IIS Deployment (PLANNED)

- [ ] Update appsettings.Production.json with production SQL Server connection
- [ ] Configure production error handling
- [ ] Set up logging (file or database)
- [ ] Publish application for IIS
- [ ] Configure IIS application pool
- [ ] Deploy to IIS server
- [ ] Run migrations on production database
- [ ] Post-deployment testing
- [ ] Document deployment process

---

## How to Run the Application

**Start the application**:
```bash
cd ReleaseTracker.Web
dotnet run
```

**Access the application**:
- URL: http://localhost:5000
- Dashboard will show summary statistics
- Navigate to Applications or Releases to start creating records

**Test the configurable dropdowns**:
1. Edit `appsettings.json`
2. Modify the `ReleaseOptions.Environments` or `ReleaseOptions.Statuses` arrays
3. Restart the application
4. Create or edit a release - dropdowns will reflect your changes!
