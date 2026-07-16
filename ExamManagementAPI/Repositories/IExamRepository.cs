using ExamManagementAPI.Models;

namespace ExamManagementAPI.Interfaces
{
    public interface IExamRepository
    {
        Task<List<ExamMaster>> GetAllWithDetailsAsync();
        Task<ExamMaster?> GetByIdWithDetailsAsync(int masterId);
        Task<bool> ExamExistsForStudentYearAsync(int studentId, int examYear);
        Task<ExamMaster> AddExamAsync(ExamMaster examMaster, List<ExamDtls> details);
        Task<ExamMaster> UpdateExamAsync(ExamMaster examMaster);
    }
}
