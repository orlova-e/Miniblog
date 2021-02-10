using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Services.UnitTests")]

namespace Services.Implementation
{
    internal static class InputPreparation
    {
        public static List<string> Prepare(string input)
        {
            List<string> preparedQuery = PrepareWithoutDistinct(input)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return preparedQuery;
        }

        public static List<string> PrepareWithoutDistinct(string input)
        {
            if (input == null)
                throw new ArgumentNullException("Input variable is null");

            List<string> preparedQuery = input
                .Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(q => Regex.Replace(q, @"[^-\w]", ""))
                .Select(s => s.ToLower())
                .Where(s => s.Length >= 4)
                .ToList();

            return preparedQuery;
        }
    }
}
