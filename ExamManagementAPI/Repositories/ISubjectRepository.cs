using ExamManagementAPI.Models;

namespace ExamManagementAPI.Interfaces
{
    // This interface lists what a "subject repository" must be able to do.
    // SubjectRepository.cs (in this same folder) provides the actual implementation.
    public interface ISubjectRepository
    {
        Task<List<SubjectMaster>> GetAllAsync(string? search);
        Task<int> CountMatchingAsync(IEnumerable<int> subjectIds);
    }
}
