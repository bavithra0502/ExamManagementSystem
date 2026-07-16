using ExamManagementAPI.DTOs;

namespace ExamManagementAPI.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentDto>> GetStudentsAsync(string? search);
        Task<StudentDto> GetStudentAsync(int studentId);
        Task<StudentDto> CreateStudentAsync(StudentCreateDto dto);
    }
}
