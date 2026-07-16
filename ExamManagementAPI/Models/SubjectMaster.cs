using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementAPI.Models
{
    // This class represents one row in the subject table.
    // [Table("SubjectMst")] keeps the actual database table name as "SubjectMst"
    // (so nothing in the existing database needs to change), while the C# class
    // itself is called "SubjectMaster" to match the naming style of "ExamMaster".
    [Table("SubjectMst")]
    public class SubjectMaster
    {
        // Primary key - auto increments in the database.
        [Key]
        public int SubjectID { get; set; }

        // Name of the subject, e.g. "Mathematics".
        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        // One subject can appear in many exam detail rows (across many students/years).
        public ICollection<ExamDtls> ExamDtls { get; set; } = new List<ExamDtls>();
    }
}
