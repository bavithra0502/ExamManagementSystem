using ExamManagementAPI.Data;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementAPI.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly ExamDbContext _context;

        public ExamRepository(ExamDbContext context)
        {
            _context = context;
        }

        private IQueryable<ExamMaster> WithDetails()
        {
            return _context.ExamMaster
                .Include(m => m.Student)
                .Include(m => m.ExamDtls)
                    .ThenInclude(d => d.Subject);
        }

        public async Task<List<ExamMaster>> GetAllWithDetailsAsync()
        {
            return await WithDetails()
                .OrderByDescending(m => m.CreateTime)
                .ToListAsync();
        }

        public async Task<ExamMaster?> GetByIdWithDetailsAsync(int masterId)
        {
            return await WithDetails().FirstOrDefaultAsync(m => m.MasterID == masterId);
        }

        public async Task<bool> ExamExistsForStudentYearAsync(int studentId, int examYear)
        {
            return await _context.ExamMaster
                .AnyAsync(m => m.StudentID == studentId && m.ExamYear == examYear);
        }

        public async Task<ExamMaster> AddExamAsync(ExamMaster examMaster, List<ExamDtls> details)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            _context.ExamMaster.Add(examMaster);
            await _context.SaveChangesAsync(); // generates MasterID

            foreach (var detail in details)
            {
                detail.MasterID = examMaster.MasterID;
                _context.ExamDtls.Add(detail);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return examMaster;
        }

        public async Task<ExamMaster> UpdateExamAsync(ExamMaster examMaster)
        {
            // examMaster is already tracked by this same DbContext (it was loaded via
            // GetByIdWithDetailsAsync in the Service layer), so saving just persists the changes.
            await _context.SaveChangesAsync();
            return examMaster;
        }
    }
}
