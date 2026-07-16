using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementAPI.Models
{
    // This class represents one row in the student table.
    // [Table("StudentMst")] keeps the actual database table name as "StudentMst"
    // (so nothing in the existing database needs to change), while the C# class
    // itself is called "StudentMaster" to match the naming style of "ExamMaster".
    [Table("StudentMst")]
    public class StudentMaster
    {
        // Primary key - auto increments in the database.
        [Key]
        public int StudentID { get; set; }

        // Student's full name. Must be between 5 and 250 characters.
        [Required]
        [MinLength(5)]
        [MaxLength(250)]
        public string StudentName { get; set; } = string.Empty;

        // Student's email. Must be unique (enforced in ExamDbContext) and a valid email format.
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;

        // One student can appear in many exam records (one per year).
        public ICollection<ExamMaster> ExamMasters { get; set; } = new List<ExamMaster>();
    }
}
