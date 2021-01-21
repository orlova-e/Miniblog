using System.Globalization;

namespace Web
{
    public static class DateTimeFormatInfoExtension
    {
        public static string RoundtripDtPattern(this DateTimeFormatInfo dateTimeFormatInfo)
        {
            return "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";
        }
        public static string RoundtripDtOffsetPattern(this DateTimeFormatInfo dateTimeFormatInfo)
        {
            return "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzz";
        }
    }
}
