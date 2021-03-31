using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;
using Web.App.Interfaces;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("icon", ParentTag = "head", TagStructure = TagStructure.WithoutEndTag)]
    public class IconTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        public string IconPath { get; set; }
        public IconTagHelper(ICommon common)
        {
            IconPath = common.Options.WebsiteOptions.IconPath;
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
