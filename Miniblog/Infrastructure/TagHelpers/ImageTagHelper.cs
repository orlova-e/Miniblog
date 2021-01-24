using Domain.Entities;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("image", TagStructure = TagStructure.WithoutEndTag)]
    public class ImageTagHelper : TagHelper
    {
        public Image Source { get; set; }
        public int HeightPx { get; set; }
        public int WidthPx { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.TagMode = TagMode.StartTagOnly;

            if (Source == null || Source.File.Length == 0)
                return;

            output.Attributes.SetAttribute("src", "data:image/jpeg;base64," + Convert.ToBase64String(Source.File));
            output.Attributes.SetAttribute("alt", Source.Name);

            if (HeightPx > 0)
            {
                output.Attributes.SetAttribute("height", HeightPx + "px");
            }
            if (WidthPx > 0)
            {
                output.Attributes.SetAttribute("width", WidthPx + "px");
            }
        }
    }
}
