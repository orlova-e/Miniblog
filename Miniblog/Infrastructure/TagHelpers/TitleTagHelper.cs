using Microsoft.AspNetCore.Razor.TagHelpers;
using Web.App.Interfaces;
using Web.Configuration;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("title", Attributes = "page", ParentTag = "head")]
    public class TitleTagHelper : TagHelper
    {
        public string Page { get; set; }
        [HtmlAttributeNotBound]
        public WebsiteOptions WebsiteOptions { get; private set; }
        public TitleTagHelper(ICommon common)
        {
            WebsiteOptions = common.Options.WebsiteOptions;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string title = WebsiteOptions.Name;

            if (!string.IsNullOrWhiteSpace(Page))
            {
                title = Page + " — " + title;
            }
            else if (!string.IsNullOrWhiteSpace(WebsiteOptions.Subtitle))
            {
                title += " — " + WebsiteOptions.Subtitle;
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetContent(title);
        }
    }
}
