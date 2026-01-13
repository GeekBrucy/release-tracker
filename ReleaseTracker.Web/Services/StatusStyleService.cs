namespace ReleaseTracker.Web.Services
{
    public interface IStatusStyleService
    {
        string GetStatusBadgeClass(string status);
    }

    public class StatusStyleService : IStatusStyleService
    {
        private readonly Dictionary<string, string> _statusColorMap;

        public StatusStyleService()
        {
            // Default color mappings
            // These can be extended to read from configuration if needed
            _statusColorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Completed", "bg-success" },
                { "In Progress", "bg-primary" },
                { "Planned", "bg-info" },
                { "Rolled Back", "bg-danger" },
                { "On Hold", "bg-warning" },
                { "Cancelled", "bg-secondary" }
            };
        }

        public string GetStatusBadgeClass(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "bg-secondary";

            // Try to find exact match first
            if (_statusColorMap.TryGetValue(status, out var colorClass))
                return colorClass;

            // Fallback: try to intelligently guess based on keywords
            var lowerStatus = status.ToLower();

            if (lowerStatus.Contains("complete") || lowerStatus.Contains("done") || lowerStatus.Contains("success"))
                return "bg-success";

            if (lowerStatus.Contains("progress") || lowerStatus.Contains("ongoing") || lowerStatus.Contains("active"))
                return "bg-primary";

            if (lowerStatus.Contains("plan") || lowerStatus.Contains("scheduled") || lowerStatus.Contains("pending"))
                return "bg-info";

            if (lowerStatus.Contains("fail") || lowerStatus.Contains("error") || lowerStatus.Contains("rollback") || lowerStatus.Contains("abort"))
                return "bg-danger";

            if (lowerStatus.Contains("hold") || lowerStatus.Contains("pause") || lowerStatus.Contains("wait"))
                return "bg-warning";

            // Default fallback
            return "bg-secondary";
        }
    }
}
