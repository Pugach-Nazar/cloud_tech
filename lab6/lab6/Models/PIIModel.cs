using System.Collections.Generic;

namespace lab6.Models
{
    public class PIIModel
    {
        public string Text { get; set; }
        public List<PII> PIIs { get; set; } = new();
    }
}
