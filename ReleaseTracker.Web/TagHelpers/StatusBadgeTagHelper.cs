using Microsoft.AspNetCore.Razor.TagHelpers;
using ReleaseTracker.Web.Services;

namespace ReleaseTracker.Web.TagHelpers
{
    [HtmlTargetElement("status-badge")]
    public class StatusBadgeTagHelper : TagHelper
    {
        private readonly IStatusStyleService _statusStyleService;

        public StatusBadgeTagHelper(IStatusStyleService statusStyleService)
        {
            _statusStyleService = statusStyleService;
        }

        [HtmlAttributeName("value")]
        public string? Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var badgeClass = _statusStyleService.GetStatusBadgeClass(Value ?? "");
            output.Attributes.SetAttribute("class", $"badge {badgeClass}");

            output.Content.SetContent(Value ?? "Unknown");
        }
    }
}
