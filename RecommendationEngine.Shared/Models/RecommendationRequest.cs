
using RecommendationEngine.Shared.Enums;

namespace RecommendationEngine.Shared.Models
{
    public class RecommendationRequest
    {
        public string EntityID { get; set; } // mandatory
        public string ProgramID { get; set; } // mandatory
        public TemplateType TemplateType { get; set; } // enumeration
        public string Region { get; set; } // mandatory
        public string Language { get; set; } // mandatory
        public string FilePath { get; set; } // mandatory (S3)
        public string FileName { get; set; } // mandatory (xlsx file check)
    }
}
