using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementAPI.Models
{
    /// <summary>Subject wise marks belonging to an ExamMaster record.</summary>
    public class ExamDtls
    {
        [Key]
        public int DtlsID { get; set; }

        [Required]
        public int MasterID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Range(0, 100)]
        public int Marks { get; set; }

        [ForeignKey(nameof(MasterID))]
        public ExamMaster? ExamMaster { get; set; }

        // Navigation property - lets us easily get the subject's name
        // for this row without a separate database query.
        [ForeignKey(nameof(SubjectID))]
        public SubjectMaster? Subject { get; set; }
    }
}
