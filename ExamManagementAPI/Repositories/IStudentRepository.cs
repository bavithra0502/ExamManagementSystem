using ExamManagementAPI.Models;

namespace ExamManagementAPI.Interfaces
{
    // This interface lists what a "student repository" must be able to do.
    // StudentRepository.cs (in this same folder) provides the actual implementation.
    public interface IStudentRepository
    {
        Task<List<StudentMaster>> GetAllAsync(string? search);
        Task<StudentMaster?> GetByIdAsync(int studentId);
        Task<bool> MailExistsAsync(string mail);
        Task<StudentMaster> AddAsync(StudentMaster student);
    }
}
