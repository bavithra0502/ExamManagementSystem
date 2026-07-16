using ExamManagementAPI.DTOs;
using ExamManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: api/Subjects?search=phy
        // Returns the subject list used to power the autofill/dropdown field on the exam entry screen.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects([FromQuery] string? search)
        {
            var subjects = await _subjectService.GetSubjectsAsync(search);
            return Ok(subjects);
        }
    }
}
