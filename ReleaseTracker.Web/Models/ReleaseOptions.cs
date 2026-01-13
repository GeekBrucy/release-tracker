namespace ReleaseTracker.Web.Models
{
    /// <summary>
    /// Configuration class for release dropdown options
    /// </summary>
    public class ReleaseOptions
    {
        public List<string> Environments { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
    }
}
