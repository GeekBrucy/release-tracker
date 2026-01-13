# Status Badge Styling - Reusable & Configurable

## Problem Solved

Previously, status badges used hardcoded `switch` statements throughout the codebase. This caused two issues:
1. **Code Duplication** - The same switch statement was repeated in multiple views
2. **Configuration Breaking** - Changing status values in `appsettings.json` would break the UI because the switch cases wouldn't match

## Solution

Created a **reusable Tag Helper** with intelligent color mapping that works with any status value.

## Architecture

### 1. StatusStyleService
**File**: [Services/StatusStyleService.cs](ReleaseTracker.Web/Services/StatusStyleService.cs)

Centralized service that maps status values to Bootstrap badge classes.

**Features**:
- ✅ Exact match mapping for known statuses
- ✅ Intelligent keyword-based fallback for custom statuses
- ✅ Case-insensitive matching
- ✅ Extensible design

**Default Mappings**:
```csharp
"Completed" → bg-success (green)
"In Progress" → bg-primary (blue)
"Planned" → bg-info (cyan)
"Rolled Back" → bg-danger (red)
"On Hold" → bg-warning (yellow)
"Cancelled" → bg-secondary (gray)
```

**Intelligent Fallback**:
If a status doesn't match exactly, it uses keyword detection:
- Contains "complete", "done", "success" → Green
- Contains "progress", "ongoing", "active" → Blue
- Contains "plan", "scheduled", "pending" → Cyan
- Contains "fail", "error", "rollback", "abort" → Red
- Contains "hold", "pause", "wait" → Yellow
- Default → Gray

### 2. StatusBadgeTagHelper
**File**: [TagHelpers/StatusBadgeTagHelper.cs](ReleaseTracker.Web/TagHelpers/StatusBadgeTagHelper.cs)

Custom Tag Helper that renders status badges with automatic color selection.

**Usage**:
```html
<!-- Old way (hardcoded, brittle) -->
@switch (release.Status)
{
    case "Completed":
        <span class="badge bg-success">@release.Status</span>
        break;
    case "In Progress":
        <span class="badge bg-primary">@release.Status</span>
        break;
    // ... 20 lines of code
}

<!-- New way (clean, flexible) -->
<status-badge value="@release.Status" />
```

**Output**:
```html
<span class="badge bg-success">Completed</span>
```

## Configuration

### Registering the Service

In [Program.cs](ReleaseTracker.Web/Program.cs):
```csharp
builder.Services.AddSingleton<IStatusStyleService, StatusStyleService>();
```

### Enabling the Tag Helper

In [Views/_ViewImports.cshtml](ReleaseTracker.Web/Views/_ViewImports.cshtml):
```cshtml
@addTagHelper *, ReleaseTracker.Web
```

## Where It's Used

The `<status-badge>` tag helper is now used in:

1. **Dashboard** - [Views/Home/Index.cshtml](ReleaseTracker.Web/Views/Home/Index.cshtml#L99)
2. **Releases List** - [Views/Releases/Index.cshtml](ReleaseTracker.Web/Views/Releases/Index.cshtml#L89)
3. **Release Details** - [Views/Releases/Details.cshtml](ReleaseTracker.Web/Views/Releases/Details.cshtml#L42)
4. **App Details** - [Views/Apps/Details.cshtml](ReleaseTracker.Web/Views/Apps/Details.cshtml#L82)

## Benefits

✅ **DRY Principle** - Status rendering logic exists in one place
✅ **Configuration-Safe** - Custom status values automatically get appropriate colors
✅ **Maintainable** - Update color logic in one service vs. multiple views
✅ **Consistent** - All status badges look and behave the same way
✅ **Extensible** - Easy to add new status types or custom color rules

## Examples

### Standard Statuses
```html
<status-badge value="Completed" />      → Green badge
<status-badge value="In Progress" />    → Blue badge
<status-badge value="Planned" />        → Cyan badge
<status-badge value="Rolled Back" />    → Red badge
```

### Custom Statuses (Intelligent Matching)
```html
<status-badge value="Successfully Deployed" />  → Green (contains "success")
<status-badge value="Deployment Failed" />      → Red (contains "fail")
<status-badge value="Awaiting Approval" />      → Cyan (contains "pending")
<status-badge value="Work In Progress" />       → Blue (contains "progress")
<status-badge value="On Hold" />                → Yellow
```

### Configurable Statuses
If you change `appsettings.json` to add new statuses:

```json
{
  "ReleaseOptions": {
    "Statuses": [
      "Scheduled",
      "Deploying",
      "Live",
      "Failed",
      "Reverted"
    ]
  }
}
```

The tag helper automatically assigns colors:
- **Scheduled** → Cyan (contains "scheduled")
- **Deploying** → Blue (active process)
- **Live** → Green (completed state)
- **Failed** → Red (contains "fail")
- **Reverted** → Red (rollback action)

## Extending the Service

### Option 1: Add Exact Mappings
Edit `StatusStyleService` constructor:

```csharp
public StatusStyleService()
{
    _statusColorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // ... existing mappings
        { "Approved", "bg-success" },
        { "Rejected", "bg-danger" },
        { "Under Review", "bg-warning" }
    };
}
```

### Option 2: Configure from appsettings.json (Future Enhancement)

You could extend this to read color mappings from configuration:

```json
{
  "StatusColorMappings": {
    "Completed": "bg-success",
    "Deployed": "bg-success",
    "Failed": "bg-danger"
  }
}
```

## Testing

### Manual Testing
1. Create a release with status "Completed" → Should show green badge
2. Edit `appsettings.json` to add status "Deployed Successfully"
3. Create a release with that status → Should show green badge (keyword match)
4. Edit `appsettings.json` to add status "Deployment Failed"
5. Create a release with that status → Should show red badge (keyword match)

### Unit Testing Example
```csharp
[Fact]
public void GetStatusBadgeClass_CompletedStatus_ReturnsSuccess()
{
    var service = new StatusStyleService();
    var result = service.GetStatusBadgeClass("Completed");
    Assert.Equal("bg-success", result);
}

[Fact]
public void GetStatusBadgeClass_CustomSuccessStatus_ReturnsSuccess()
{
    var service = new StatusStyleService();
    var result = service.GetStatusBadgeClass("Deployment Successful");
    Assert.Equal("bg-success", result); // Keyword match
}
```

## Migration Guide

If you need to add status rendering to new views:

**Before**:
```html
@if (item.Status == "Completed")
{
    <span class="badge bg-success">@item.Status</span>
}
else if (item.Status == "Failed")
{
    <span class="badge bg-danger">@item.Status</span>
}
```

**After**:
```html
<status-badge value="@item.Status" />
```

**Benefits**: 1 line instead of 15-20, automatic color selection, configuration-safe.

## Best Practices

1. **Always use `<status-badge>`** for rendering release statuses
2. **Don't hardcode status colors** in views
3. **Use descriptive status names** - The keyword matching works better with clear names
4. **Test custom statuses** - Add a custom status and verify it gets an appropriate color
5. **Update StatusStyleService** if you need precise control over custom status colors

## Troubleshooting

**Issue**: Custom status shows gray badge instead of expected color
**Solution**: Check if the status name contains keywords that match the fallback logic. If not, add an exact mapping in `StatusStyleService`.

**Issue**: All statuses show the same color
**Solution**: Verify that `StatusStyleService` is registered in `Program.cs` and Tag Helper is enabled in `_ViewImports.cshtml`.

**Issue**: Tag helper not recognized
**Solution**: Ensure you have `@addTagHelper *, ReleaseTracker.Web` in `_ViewImports.cshtml` and rebuild the project.
