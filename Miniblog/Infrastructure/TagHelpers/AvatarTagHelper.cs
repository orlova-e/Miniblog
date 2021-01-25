using Domain.Entities;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Web.Configuration;

namespace Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("avatar", TagStructure = TagStructure.WithoutEndTag)]
    public class AvatarTagHelper : TagHelper
    {
        public User User { get; set; }
        public string Username { get; set; }
        public byte[] Image { get; set; }
        public int? Px { get; set; } = 100;
        [HtmlAttributeNotBound]
        public string ImagePath { get; set; }

        public AvatarTagHelper(IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            string path = optionsSnapshot.Value.WebsiteOptions.StandardAvatarPath;
            ImagePath = Path.Combine("~", path);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.TagMode = TagMode.StartTagOnly;

            if (User == null)
                User = new User { Username = Username, Avatar = Image };

            if (User?.Avatar?.Length > 0)
            {
                output.Attributes.SetAttribute("src", "data:image/jpeg;base64," + Convert.ToBase64String(User.Avatar));
                output.Attributes.SetAttribute("alt", User.Username ?? string.Empty);
            }
            else
            {
                output.Attributes.SetAttribute("src", ImagePath);
                output.Attributes.SetAttribute("alt", "User");
            }

            output.Attributes.SetAttribute("height", Px + "px");
            output.Attributes.SetAttribute("width", Px + "px");
        }
    }
}
