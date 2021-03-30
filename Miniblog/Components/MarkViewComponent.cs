using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Net;
using System;

namespace Web.Components
{
    public class MarkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string text, List<string> matches)
        {
            text = WebUtility.HtmlEncode(text);

            foreach (string match in matches)
            {
                if (!text.Contains(match, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int start = 0, current = 0;
                do
                {
                    current = start;
                    start = text.IndexOf(match, start, StringComparison.OrdinalIgnoreCase);
                    text = text.Substring(0, start) + "<mark>" + text.Substring(start, match.Length) + "</mark>" + text.Substring(start + match.Length);
                    start += "<mark></mark>".Length;
                } while (start > current && text.IndexOf(match, start, StringComparison.OrdinalIgnoreCase) > -1);
            }

            IHtmlContent content = new HtmlString(text);
            return new HtmlContentViewComponentResult(content);
        }
    }
}
