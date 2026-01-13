using System.ComponentModel.DataAnnotations;

namespace ReleaseTracker.Web.Models
{
    public class App
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Application name is required")]
        [StringLength(255, ErrorMessage = "Application name cannot exceed 255 characters")]
        [Display(Name = "Application Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Release> Releases { get; set; } = new List<Release>();
    }
}
