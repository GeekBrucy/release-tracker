using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTracker.Web.Models
{
    public class Release
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Application is required")]
        [Display(Name = "Application")]
        public int AppId { get; set; }

        [Required(ErrorMessage = "Version is required")]
        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        [Display(Name = "Version")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release date is required")]
        [Display(Name = "Release Date")]
        [DataType(DataType.DateTime)]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Released by is required")]
        [StringLength(255, ErrorMessage = "Released by cannot exceed 255 characters")]
        [Display(Name = "Released By")]
        public string ReleasedBy { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Release Notes")]
        [DataType(DataType.MultilineText)]
        public string? ReleaseNotes { get; set; }

        [Required(ErrorMessage = "Environment is required")]
        [StringLength(50)]
        [Display(Name = "Environment")]
        public string Environment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        // Navigation property
        [ForeignKey("AppId")]
        public App? App { get; set; }
    }
}
