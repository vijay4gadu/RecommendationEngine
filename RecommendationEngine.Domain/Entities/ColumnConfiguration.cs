
namespace RecommendationEngine.Domain.Entities
{
    public class ColumnConfiguration 
    {
        public string ColumnId { get; set; } // Maps to column_id
        public int Index { get; set; } // Maps to index
        public string Name { get; set; } // Maps to name
        public string ColumnHeader { get; set; } // Maps to column_header
        public string RegexExp { get; set; } // Maps to regex_exp
        public string RegexErrorMessage { get; set; } // Maps to regex_error_message
        public bool Mandatory { get; set; } // Maps to mandatory (bit)
        public string MandatoryErrorMessage { get; set; } // Maps to mandatory_error_message
        public int Precision { get; set; } // Maps to precision (integer)
        public string TemplateId { get; set; } // Maps to template_id
    }
   
}
