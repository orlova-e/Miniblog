using Services.Interfaces;
using System;
using System.Net;
using System.Reflection;

namespace Services.Implementation
{
    public class TextService : ITextService
    {
        public string GetEncoded(string text)
        {
            string newText = WebUtility.HtmlEncode(text);
            return newText;
        }

        public object GetEncoded(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public);
            for(int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetValue(obj) is string)
                {
                    properties[i].SetValue(obj, WebUtility.HtmlEncode(properties[i].GetValue(obj).ToString()));
                }
            }
            return obj;
        }

        public string FixLines(string text)
        {
            string[] paragraphs = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            text = string.Join(Environment.NewLine, paragraphs);
            return text;
        }

        public string GetPrepared(string text)
        {
            text = GetEncoded(text);
            text = FixLines(text);
            return text;
        }
    }
}
