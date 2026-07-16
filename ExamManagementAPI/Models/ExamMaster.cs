using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementAPI.Models
{
    /// <summary>One record per Student + ExamYear. TotalMark and PassOrFail are calculated fields.</summary>
    public class ExamMaster
    {
        [Key]
        public int MasterID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int ExamYear { get; set; }

        // Calculated: sum of ExamDtls.Marks
        public int TotalMark { get; set; }

        // Calculated: "PASS" if every subject's Marks >= 25, otherwise "FAIL"
        [MaxLength(10)]
        public string PassOrFail { get; set; } = "FAIL";

        public DateTime CreateTime { get; set; } = DateTime.Now;

        // Navigation property - lets us easily get the student's name/email
        // for this exam record without a separate database query.
        [ForeignKey(nameof(StudentID))]
        public StudentMaster? Student { get; set; }

        public ICollection<ExamDtls> ExamDtls { get; set; } = new List<ExamDtls>();
    }
}
