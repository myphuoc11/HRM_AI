using System;
using System.Collections.Generic;

namespace HRM_AI.Services.Helpers
{
    public static class PlaceholderReplacer
    {

        public static string Replace(string template, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(template) || replacements == null)
                return template;

            foreach (var kvp in replacements)
            {
                if (string.IsNullOrEmpty(kvp.Key)) continue;

                var placeholder1 = $"{{{{{kvp.Key}}}}}"; // {{CandidateName}}
                var placeholder2 = $"{{{kvp.Key}}}";     // {CandidateName}
                template = template
                    .Replace(placeholder1, kvp.Value ?? string.Empty)
                    .Replace(placeholder2, kvp.Value ?? string.Empty);
            }

            return template;
        }

    }
}
