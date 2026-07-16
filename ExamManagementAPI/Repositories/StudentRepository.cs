using ExamManagementAPI.Data;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementAPI.Repositories
{
    // This class talks directly to the database for anything related to students.
    // It has no validation logic - that lives in StudentService. This class only
    // knows how to read/write student rows.
    public class StudentRepository : IStudentRepository
    {
        private readonly ExamDbContext _context;

        public StudentRepository(ExamDbContext context)
        {
            _context = context;
        }

        // Returns all students, optionally filtered by name or email.
        public async Task<List<StudentMaster>> GetAllAsync(string? search)
        {
            var query = _context.StudentMaster.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.StudentName.Contains(search) || s.Mail.Contains(search));
            }

            return await query.OrderBy(s => s.StudentName).ToListAsync();
        }

        // Finds one student by their ID. Returns null if not found.
        public async Task<StudentMaster?> GetByIdAsync(int studentId)
        {
            return await _context.StudentMaster.FindAsync(studentId);
        }

        // Checks if a given email is already used by another student.
        public async Task<bool> MailExistsAsync(string mail)
        {
            return await _context.StudentMaster.AnyAsync(s => s.Mail == mail);
        }

        // Adds a new student row to the database.
        public async Task<StudentMaster> AddAsync(StudentMaster student)
        {
            _context.StudentMaster.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }
    }
}
