using ExamManagementAPI.DTOs;
using ExamManagementAPI.Helpers;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Models;

namespace ExamManagementAPI.Services
{
    // This class holds the "business rules" for students: name length, unique email, etc.
    // It talks to the database only through IStudentRepository - never directly.
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        // Returns the student list shown in the dropdown on the exam entry screen.
        public async Task<List<StudentDto>> GetStudentsAsync(string? search)
        {
            var students = await _studentRepository.GetAllAsync(search);
            return students.Select(MapToDto).ToList();
        }

        // Returns one student by ID, or throws a "not found" error.
        public async Task<StudentDto> GetStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                throw new NotFoundException($"Student with ID {studentId} was not found.");
            }

            return MapToDto(student);
        }

        // Creates a new student, after checking the name length and that the email is unique.
        public async Task<StudentDto> CreateStudentAsync(StudentCreateDto dto)
        {
            var name = (dto.StudentName ?? string.Empty).Trim();
            var mail = (dto.Mail ?? string.Empty).Trim();

            if (name.Length < 5 || name.Length > 250)
            {
                throw new ValidationException("Student name must be between 5 and 250 characters.");
            }

            if (string.IsNullOrWhiteSpace(mail))
            {
                throw new ValidationException("Email is required.");
            }

            if (await _studentRepository.MailExistsAsync(mail))
            {
                throw new ConflictException($"A student with email '{mail}' already exists.");
            }

            var student = new StudentMaster { StudentName = name, Mail = mail };
            var created = await _studentRepository.AddAsync(student);

            return MapToDto(created);
        }

        // Converts a StudentMaster (database entity) into a StudentDto (what the API returns).
        // Keeping these separate means the JSON shape the Angular app sees never
        // has to change, even if we rename or restructure the database entity.
        private static StudentDto MapToDto(StudentMaster s) => new()
        {
            StudentID = s.StudentID,
            StudentName = s.StudentName,
            Mail = s.Mail
        };
    }
}
