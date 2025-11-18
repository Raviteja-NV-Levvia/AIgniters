using Microsoft.AspNetCore.Mvc;
using AIPoweredDefectManagementAssistant.Services.FileService;
using AIPoweredDefectManagementAssistant.Models;

namespace AIPoweredDefectManagementAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefectsController : ControllerBase
    {
        private readonly ExcelService _excelService;

        public DefectsController(ExcelService excelService)
        {
            _excelService = excelService;
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            var defects = await _excelService.ReadExcelAsync(file.OpenReadStream());
            return Ok(new
            {
                message = $"✅ Loaded {defects.Count} test cases successfully.",
                defects
            });
        }
    }
}
