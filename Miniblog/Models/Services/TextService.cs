using Miniblog.Models.Services.Interfaces;
using System;
using System.Net;
using System.Reflection;

namespace Miniblog.Models.Services
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
    }
}
