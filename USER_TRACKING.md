# User Tracking Implementation

## Overview

The application now automatically captures the current logged-in user for audit tracking in a **cross-platform, OS-agnostic** way.

## How It Works

### UserService Implementation

The `UserService` class provides a centralized way to get the current user's name, with multiple fallback strategies:

**Priority Order:**
1. **ASP.NET Identity** - If Windows Authentication or ASP.NET Core Identity is configured
2. **Environment.UserName** - Falls back to OS-level username (works on Windows, Linux, macOS)
3. **"System"** - Final fallback if no user can be determined

**File**: [Services/UserService.cs](ReleaseTracker.Web/Services/UserService.cs)

```csharp
public string GetCurrentUserName()
{
    // Try ASP.NET Identity first
    var identityName = httpContext.User?.Identity?.Name;
    if (!string.IsNullOrEmpty(identityName))
    {
        // Clean up Windows domain format (DOMAIN\username -> username)
        if (identityName.Contains('\\'))
        {
            var parts = identityName.Split('\\');
            return parts[parts.Length - 1];
        }
        return identityName;
    }

    // Fallback to environment variable (OS-agnostic)
    var envUser = Environment.UserName;
    if (!string.IsNullOrEmpty(envUser))
        return envUser;

    // Last resort
    return "System";
}
```

## Configuration

### For IIS Deployment with Windows Authentication

1. **Enable Windows Authentication in IIS**:
   - Open IIS Manager
   - Select your application
   - Open "Authentication" feature
   - Enable "Windows Authentication"
   - Disable "Anonymous Authentication" (optional but recommended)

2. **Update Program.cs** (if not already added):
   ```csharp
   builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
   ```

3. **Users will be automatically tracked** using their Windows login (e.g., "jsmith" from "DOMAIN\jsmith")

### For Development (No Authentication)

- The service will use `Environment.UserName`
- On Windows: Your Windows username
- On macOS: Your macOS username
- On Linux: Your Linux username

This is **OS-agnostic** and works without any configuration!

### For Custom Authentication

If you implement custom authentication (e.g., ASP.NET Core Identity, OAuth, etc.), the service will automatically use the authenticated user's name from `User.Identity.Name`.

## Usage in Controllers

The service is injected via dependency injection:

```csharp
public class AppsController : Controller
{
    private readonly IUserService _userService;

    public AppsController(ReleaseTrackerContext context, IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Create(App app)
    {
        app.CreatedBy = _userService.GetCurrentUserName();
        // ... save to database
    }
}
```

## Where User Tracking is Used

### Apps
- **CreatedBy**: Set when an application is created
- Captured in: [AppsController.cs](ReleaseTracker.Web/Controllers/AppsController.cs#L72)

### Releases
- **CreatedBy**: Set when a release is created
- **ModifiedBy**: Set when a release is edited
- Captured in:
  - Create: [ReleasesController.cs](ReleaseTracker.Web/Controllers/ReleasesController.cs#L105)
  - Edit: [ReleasesController.cs](ReleaseTracker.Web/Controllers/ReleasesController.cs#L151)

## Benefits

✅ **OS-Agnostic**: Works on Windows, Linux, and macOS
✅ **No Configuration Needed**: Works out of the box in development
✅ **IIS-Ready**: Seamlessly integrates with Windows Authentication
✅ **Flexible**: Supports custom authentication if implemented later
✅ **Consistent**: Single source of truth for user identification
✅ **Testable**: Interface-based design allows easy mocking for unit tests

## Testing

### Development Mode
When running locally with `dotnet run`, the username will be your OS login name.

**Example on macOS/Linux**:
```bash
$ whoami
someuser

# When you create an app, CreatedBy will be "someuser"
```

**Example on Windows**:
```
C:\> whoami
DESKTOP-ABC123\JohnDoe

# When you create an app, CreatedBy will be "JohnDoe" (domain stripped)
```

### Production (IIS with Windows Authentication)
Users will be tracked using their Active Directory username automatically.

**Example**:
- User logs in as `COMPANY\jane.smith`
- Application stores: `jane.smith` (domain prefix removed)

## Future Enhancements

If needed, you can extend the `UserService` to:
- Include full name instead of username
- Store email address
- Integrate with LDAP/Active Directory for user lookup
- Add caching for performance optimization

## Troubleshooting

**Issue**: User shows as "System"
**Solution**:
- In development: This is normal if authentication is not configured
- In production: Check that Windows Authentication is enabled in IIS

**Issue**: Username shows as "DOMAIN\username" instead of just "username"
**Solution**: The UserService already handles this by stripping the domain prefix. If you see this, check that the service is being used correctly.

**Issue**: Want to show full name instead of username
**Solution**: Extend the UserService to query Active Directory or a user database for the full name based on the username.
