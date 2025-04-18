using System.Collections.Generic;

namespace lab6.Models
{
    public class HealthTextModel
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public List<PII> PIIs { get; set; }
        public List<MedicalEntity> MedicalEntities { get; set; }
        public List<MedicalRelation> MedicalRelations { get; set; }
    }

    public class MedicalRelation
    {
        public string RelationType { get; set; }
        public List<MedicalRole> Roles { get; set; } = new();
    }
    public class MedicalRole
    {
        public string RoleName { get; set; }
        public string EntityText { get; set; }
        public string EntityCategory { get; set; }
    }

    public class MedicalEntity
    {
        public string Text { get; set; }
        public string Category { get; set; }
        public double ConfidenceScore { get; set; }
    }

    public class PII
    {
        public string Text { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public double ConfidenceScore { get; set; }
    }
}
