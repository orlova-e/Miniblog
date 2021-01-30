using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Web.Configuration;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("title", Attributes = "page", ParentTag = "head")]
    public class TitleTagHelper : TagHelper
    {
        public string Page { get; set; }
        [HtmlAttributeNotBound]
        public WebsiteOptions WebsiteOptions { get; private set; }
        public TitleTagHelper(IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            WebsiteOptions = optionsSnapshot.Value.WebsiteOptions;
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
