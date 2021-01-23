using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.IO;
using Web.Configuration;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("icon", ParentTag = "head", TagStructure = TagStructure.WithoutEndTag)]
    public class IconTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        public string IconPath { get; set; }
        public IconTagHelper(IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            IconPath = optionsSnapshot.Value.WebsiteOptions.IconPath;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "link";
            output.TagMode = TagMode.StartTagOnly;
            output.Attributes.SetAttribute("rel", "shortcut icon");
            output.Attributes.SetAttribute("href", Path.Combine("~", IconPath));
        }
    }
}
