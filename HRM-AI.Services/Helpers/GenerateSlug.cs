using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HRM_AI.Services.Helpers
{
    public class GenerateSlug
    {
        public static string CreateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            return Regex.Replace(input.ToLower(), @"[^a-z0-9\s-]", "")
                         .Replace(" ", "-");
        }
    }
}
