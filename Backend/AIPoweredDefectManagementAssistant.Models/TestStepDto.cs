using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPoweredDefectManagementAssistant.Models
{
    public class TestStepDto
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Defect")]
        public string TestCaseId { get; set; }
        public Defect? Defect { get; set; }
        public int StepNumber { get; set; }
        public string TestStep { get; set; }
        public string TestData { get; set; }
        public string ExpectedResult { get; set; }
        public string Screenshot { get; set; }
        public string ExecutionStatus { get; set; }
    }

}
