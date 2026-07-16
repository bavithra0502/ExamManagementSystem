using ExamManagementAPI.DTOs;
using ExamManagementAPI.Helpers;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementAPI.Services
{
    public class ExamService : IExamService
    {
        private const int PassMark = 25;

        private readonly IExamRepository _examRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ISubjectRepository _subjectRepository;

        public ExamService(
            IExamRepository examRepository,
            IStudentRepository studentRepository,
            ISubjectRepository subjectRepository)
        {
            _examRepository = examRepository;
            _studentRepository = studentRepository;
            _subjectRepository = subjectRepository;
        }

        public async Task<List<ExamResultDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAllWithDetailsAsync();
            return exams.Select(MapToDto).ToList();
        }

        public async Task<ExamResultDto> GetExamAsync(int masterId)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(masterId);
            if (exam == null)
            {
                throw new NotFoundException($"Exam record with ID {masterId} was not found.");
            }

            return MapToDto(exam);
        }

        public async Task<ExamResultDto> SaveExamAsync(ExamSaveRequestDto request)
        {
            // 1. Student must exist
            var student = await _studentRepository.GetByIdAsync(request.StudentID);
            if (student == null)
            {
                throw new ValidationException("Selected student does not exist. Please choose a student from the list.");
            }

            // 2. At least one subject row
            if (request.Subjects == null || request.Subjects.Count == 0)
            {
                throw new ValidationException("Please add at least one subject with marks before saving.");
            }

            // 3. No duplicate subjects in the same submission
            var duplicateSubjectIds = request.Subjects
                .GroupBy(s => s.SubjectID)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateSubjectIds.Any())
            {
                throw new ValidationException("The same subject has been added more than once. Please remove the duplicate row.");
            }

            // 4. All subjects must be valid, and marks in range 0-100 (the [Range] attribute already
            //    checks this, but we re-validate defensively in case model binding is bypassed)
            var subjectIds = request.Subjects.Select(s => s.SubjectID).ToList();
            var validSubjectCount = await _subjectRepository.CountMatchingAsync(subjectIds);
            if (validSubjectCount != subjectIds.Distinct().Count())
            {
                throw new ValidationException("One or more selected subjects are invalid. Please choose subjects from the list.");
            }

            foreach (var line in request.Subjects)
            {
                if (line.Marks < 0 || line.Marks > 100)
                {
                    throw new ValidationException($"Marks for subject ID {line.SubjectID} must be between 0 and 100.");
                }
            }

            // 5. StudentID + ExamYear must be unique
            var alreadyExists = await _examRepository.ExamExistsForStudentYearAsync(request.StudentID, request.ExamYear);
            if (alreadyExists)
            {
                throw new ConflictException($"An exam record already exists for {student.StudentName} for the year {request.ExamYear}.");
            }

            // 6. Calculate TotalMark and PassOrFail
            var totalMark = request.Subjects.Sum(s => s.Marks);
            var passOrFail = request.Subjects.All(s => s.Marks >= PassMark) ? "PASS" : "FAIL";

            var examMaster = new ExamMaster
            {
                StudentID = request.StudentID,
                ExamYear = request.ExamYear,
                TotalMark = totalMark,
                PassOrFail = passOrFail,
                CreateTime = DateTime.Now
            };

            var details = request.Subjects.Select(s => new ExamDtls
            {
                SubjectID = s.SubjectID,
                Marks = s.Marks
            }).ToList();

            ExamMaster saved;
            try
            {
                saved = await _examRepository.AddExamAsync(examMaster, details);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("Could not save the exam record. It may already exist, please refresh and try again.");
            }

            var fullRecord = await _examRepository.GetByIdWithDetailsAsync(saved.MasterID);
            return MapToDto(fullRecord!);
        }

        public async Task<ExamResultDto> UpdateExamAsync(int masterId, UpdateExamRequestDto request)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(masterId);
            if (exam == null)
            {
                throw new NotFoundException($"Exam record with ID {masterId} was not found.");
            }

            if (request.Subjects == null || request.Subjects.Count == 0)
            {
                throw new ValidationException("Please provide marks for at least one subject.");
            }

            foreach (var line in request.Subjects)
            {
                if (line.Marks < 0 || line.Marks > 100)
                {
                    throw new ValidationException($"Marks for subject ID {line.SubjectID} must be between 0 and 100.");
                }

                var existingDetail = exam.ExamDtls.FirstOrDefault(d => d.SubjectID == line.SubjectID);
                if (existingDetail == null)
                {
                    throw new ValidationException("Subjects cannot be added or removed while editing, only marks can change.");
                }

                existingDetail.Marks = line.Marks;
            }

            // recalculate the same way SaveExamAsync does
            exam.TotalMark = exam.ExamDtls.Sum(d => d.Marks);
            exam.PassOrFail = exam.ExamDtls.All(d => d.Marks >= PassMark) ? "PASS" : "FAIL";

            var updated = await _examRepository.UpdateExamAsync(exam);
            return MapToDto(updated);
        }

        private static ExamResultDto MapToDto(ExamMaster m)
        {
            return new ExamResultDto
            {
                MasterID = m.MasterID,
                StudentID = m.StudentID,
                StudentName = m.Student?.StudentName ?? string.Empty,
                Mail = m.Student?.Mail ?? string.Empty,
                ExamYear = m.ExamYear,
                TotalMark = m.TotalMark,
                PassOrFail = m.PassOrFail,
                CreateTime = m.CreateTime,
                Subjects = m.ExamDtls.Select(d => new ExamDtlResultDto
                {
                    DtlsID = d.DtlsID,
                    SubjectID = d.SubjectID,
                    SubjectName = d.Subject?.SubjectName ?? string.Empty,
                    Marks = d.Marks
                }).ToList()
            };
        }
    }
}
