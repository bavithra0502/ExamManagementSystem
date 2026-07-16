using ExamManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementAPI.Data
{
    // This class is the bridge between our C# code and the SQL Server database.
    // Each DbSet<T> below represents one table. EF Core uses this class to
    // translate our C# code (LINQ queries, .Add(), .SaveChangesAsync(), etc.)
    // into actual SQL statements behind the scenes.
    public class ExamDbContext : DbContext
    {
        public ExamDbContext(DbContextOptions<ExamDbContext> options) : base(options)
        {
        }

        // One DbSet per table in the database.
        public DbSet<SubjectMaster> SubjectMaster => Set<SubjectMaster>();
        public DbSet<StudentMaster> StudentMaster => Set<StudentMaster>();
        public DbSet<ExamMaster> ExamMaster => Set<ExamMaster>();
        public DbSet<ExamDtls> ExamDtls => Set<ExamDtls>();

        // This method runs once when the app starts, to configure extra rules
        // (unique constraints, relationships between tables) and to seed sample data.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- StudentMaster ----
            // A student's email must be unique - no two students can share the same email.
            modelBuilder.Entity<StudentMaster>()
                .HasIndex(s => s.Mail)
                .IsUnique();

            // ---- ExamMaster ----
            // A student can only have ONE exam record per year (StudentID + ExamYear must be unique).
            modelBuilder.Entity<ExamMaster>()
                .HasIndex(e => new { e.StudentID, e.ExamYear })
                .IsUnique();

            // Each ExamMaster record belongs to one student.
            modelBuilder.Entity<ExamMaster>()
                .HasOne(e => e.Student)
                .WithMany(s => s.ExamMasters)
                .HasForeignKey(e => e.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- ExamDtls ----
            // The same subject can't appear twice under the same exam record.
            modelBuilder.Entity<ExamDtls>()
                .HasIndex(d => new { d.MasterID, d.SubjectID })
                .IsUnique();

            // Each ExamDtls row belongs to one ExamMaster record.
            // If the ExamMaster record is deleted, its ExamDtls rows are deleted too (Cascade).
            modelBuilder.Entity<ExamDtls>()
                .HasOne(d => d.ExamMaster)
                .WithMany(m => m.ExamDtls)
                .HasForeignKey(d => d.MasterID)
                .OnDelete(DeleteBehavior.Cascade);

            // Each ExamDtls row belongs to one subject.
            modelBuilder.Entity<ExamDtls>()
                .HasOne(d => d.Subject)
                .WithMany(s => s.ExamDtls)
                .HasForeignKey(d => d.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Seed data (mirrors the SQL script sample data) ----
            // This inserts a few sample subjects automatically, so the app has data to work with.
            modelBuilder.Entity<SubjectMaster>().HasData(
                new SubjectMaster { SubjectID = 1, SubjectName = "Mathematics" },
                new SubjectMaster { SubjectID = 2, SubjectName = "Physics" },
                new SubjectMaster { SubjectID = 3, SubjectName = "Chemistry" },
                new SubjectMaster { SubjectID = 4, SubjectName = "English" },
                new SubjectMaster { SubjectID = 5, SubjectName = "Computer Science" },
                new SubjectMaster { SubjectID = 6, SubjectName = "Biology" }
            );

            // This inserts a few sample students automatically, so the app has data to work with.
            modelBuilder.Entity<StudentMaster>().HasData(
                new StudentMaster { StudentID = 1, StudentName = "Arun Kumar", Mail = "arun.kumar@example.com" },
                new StudentMaster { StudentID = 2, StudentName = "Divya Menon", Mail = "divya.menon@example.com" },
                new StudentMaster { StudentID = 3, StudentName = "Rahul Nair", Mail = "rahul.nair@example.com" },
                new StudentMaster { StudentID = 4, StudentName = "Sneha Pillai", Mail = "sneha.pillai@example.com" },
                new StudentMaster { StudentID = 5, StudentName = "Vishnu Prasad", Mail = "vishnu.prasad@example.com" }
            );
        }
    }
}
