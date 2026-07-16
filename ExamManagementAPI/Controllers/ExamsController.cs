using ExamManagementAPI.DTOs;
using ExamManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        // GET: api/Exams
        // Returns every saved ExamMaster record together with its subject/marks breakdown,
        // used to populate the "saved student information" table on the screen.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamResultDto>>> GetExams()
        {
            var results = await _examService.GetAllExamsAsync();
            return Ok(results);
        }

        // GET: api/Exams/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExamResultDto>> GetExam(int id)
        {
            var exam = await _examService.GetExamAsync(id);
            return Ok(exam);
        }

        // POST: api/Exams
        // Saves ExamMaster + ExamDtls in a single transaction after validating business rules.
        // All validation lives in ExamService; failures are converted to the right status code
        // (400/409) by the exception middleware, with the same messages as before.
        [HttpPost]
        public async Task<ActionResult<ExamResultDto>> SaveExam([FromBody] ExamSaveRequestDto request)
        {
            var saved = await _examService.SaveExamAsync(request);
            return CreatedAtAction(nameof(GetExam), new { id = saved.MasterID }, saved);
        }

        // PUT: api/Exams/5
        // Edits an existing exam record - only marks can be changed, student/year/subjects stay fixed.
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExamResultDto>> UpdateExam(int id, [FromBody] UpdateExamRequestDto request)
        {
            var updated = await _examService.UpdateExamAsync(id, request);
            return Ok(updated);
        }
    }
}
