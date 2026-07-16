using ExamManagementAPI.DTOs;
using ExamManagementAPI.Interfaces;

namespace ExamManagementAPI.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<List<SubjectDto>> GetSubjectsAsync(string? search)
        {
            var subjects = await _subjectRepository.GetAllAsync(search);
            return subjects.Select(s => new SubjectDto
            {
                SubjectID = s.SubjectID,
                SubjectName = s.SubjectName
            }).ToList();
        }
    }
}
