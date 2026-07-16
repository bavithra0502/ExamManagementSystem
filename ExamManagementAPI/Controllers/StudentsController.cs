using ExamManagementAPI.DTOs;
using ExamManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Students?search=ar
        // Returns the student list used to power the autofill/dropdown field on the exam entry screen.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents([FromQuery] string? search)
        {
            var students = await _studentService.GetStudentsAsync(search);
            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentAsync(id);
            return Ok(student);
        }

        // POST: api/Students
        // Included for completeness / testing - StudentName length and unique Mail are validated
        // in the service layer; failures are converted to the right status code by the exception middleware.
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] StudentCreateDto dto)
        {
            var created = await _studentService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetStudent), new { id = created.StudentID }, created);
        }
    }
}
