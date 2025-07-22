using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Repositories.Models.ParsedFieldModels
{
    public class ParsedFieldModel
    {
        public string Type { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int GroupIndex { get; set; }
    }
}
