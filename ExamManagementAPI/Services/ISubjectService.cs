using ExamManagementAPI.DTOs;

namespace ExamManagementAPI.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetSubjectsAsync(string? search);
    }
}
