using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPoweredDefectManagementAssistant.Models
{
    public class Defect
    {
        [Key]
        public string TestCaseId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Module { get; set; }
        public string ExpectedResult { get; set; }
        public string ActualResult { get; set; }
        public string Status { get; set; }

        public List<TestStepDto> Steps { get; set; } = new();

        // AI generated fields
        public string GeneratedDescription { get; set; }
        public string GeneratedStepsToReproduce { get; set; }
        public string SuggestedSeverity { get; set; }
        public string SuggestedPriority { get; set; }

        public float[] Embedding { get; set; }
    }

}
