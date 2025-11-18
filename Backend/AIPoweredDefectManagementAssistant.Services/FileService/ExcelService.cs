using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIPoweredDefectManagementAssistant.Models;
using ClosedXML.Excel;

namespace AIPoweredDefectManagementAssistant.Services.FileService
{
    public class ExcelService
    {
        public async Task<List<Defect>> ReadExcelAsync(Stream excelStream)
        {
            var defects = new List<Defect>();

            using var workbook = new XLWorkbook(excelStream);
            var masterSheet = workbook.Worksheet("Test Case Report");

            if (masterSheet == null)
                throw new Exception("The Excel file must contain a 'Test Case Report' sheet.");

            // Read main sheet rows (skip header)
            var rows = masterSheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var status = row.Cell(5).GetString()?.Trim();

                // ✅ Only include failed test cases
                if (!string.Equals(status, "Fail", StringComparison.OrdinalIgnoreCase))
                    continue;

                var defect = new Defect
                {
                    TestCaseId = row.Cell(1).GetString(),
                    Title = row.Cell(2).GetString(),
                    ExpectedResult = row.Cell(3).GetString(),
                    ActualResult = row.Cell(4).GetString(),
                    Status = status,
                    Module = row.Cell(6).GetString(),
                    Steps = new List<TestStepDto>()
                };

                // 🔹 Read the corresponding steps sheet (if it exists)
                if (workbook.Worksheets.TryGetWorksheet(defect.TestCaseId, out var stepsSheet))
                {
                    var stepRows = stepsSheet.RangeUsed()?.RowsUsed().Skip(1);
                    if (stepRows != null)
                    {
                        foreach (var stepRow in stepRows)
                        {
                            var step = new TestStepDto
                            {
                                StepNumber = stepRow.Cell(1).GetValue<int>(),
                                TestStep = stepRow.Cell(2).GetString(),
                                TestData = stepRow.Cell(3).GetString(),
                                ExpectedResult = stepRow.Cell(4).GetString(),
                                Screenshot = stepRow.Cell(5).GetString(),
                                ExecutionStatus = stepRow.Cell(6).GetString()
                            };
                            defect.Steps.Add(step);
                        }
                    }
                }

                defects.Add(defect);
            }

            await Task.CompletedTask;
            return defects;
        }
    }
}
