using ExamManagementAPI.DTOs;

namespace ExamManagementAPI.Interfaces
{
    public interface IExamService
    {
        Task<List<ExamResultDto>> GetAllExamsAsync();
        Task<ExamResultDto> GetExamAsync(int masterId);
        Task<ExamResultDto> SaveExamAsync(ExamSaveRequestDto request);
        Task<ExamResultDto> UpdateExamAsync(int masterId, UpdateExamRequestDto request);
    }
}
