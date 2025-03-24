using System.Collections.Generic;

namespace lab5.Models
{
    public class TextInputModel
    {
        public string Text { get; set; }
        public List<EntityModel> Entities { get; set; } = new();
    }

    public class EntityModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DataSource { get; set; }
        public List<EntityMatch> Matches { get; set; } = new();
    }

    public class EntityMatch
    {
        public string Text { get; set; }
        public double ConfidenceScore { get; set; }
    }
}
