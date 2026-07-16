using ExamManagementAPI.Data;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementAPI.Repositories
{
    // This class talks directly to the database for anything related to subjects.
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ExamDbContext _context;

        public SubjectRepository(ExamDbContext context)
        {
            _context = context;
        }

        // Returns all subjects, optionally filtered by name.
        public async Task<List<SubjectMaster>> GetAllAsync(string? search)
        {
            var query = _context.SubjectMaster.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.SubjectName.Contains(search));
            }

            return await query.OrderBy(s => s.SubjectName).ToListAsync();
        }

        // Counts how many of the given subject IDs actually exist in the database.
        // Used to check that every subject the user submitted is valid.
        public async Task<int> CountMatchingAsync(IEnumerable<int> subjectIds)
        {
            var ids = subjectIds.ToList();
            return await _context.SubjectMaster.CountAsync(s => ids.Contains(s.SubjectID));
        }
    }
}
