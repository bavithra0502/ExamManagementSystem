using System.ComponentModel.DataAnnotations;

namespace ExamManagementAPI.DTOs
{
    /// <summary>One subject/marks line submitted from the Angular screen.</summary>
    public class ExamDtlRequestDto
    {
        [Required]
        public int SubjectID { get; set; }

        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100.")]
        public int Marks { get; set; }
    }

    /// <summary>Full payload posted when the user clicks "Save".</summary>
    public class ExamSaveRequestDto
    {
        [Required]
        public int StudentID { get; set; }

        [Required]
        [Range(1900, 2200, ErrorMessage = "Please provide a valid exam year.")]
        public int ExamYear { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one subject and mark must be added.")]
        public List<ExamDtlRequestDto> Subjects { get; set; } = new();
    }

    /// <summary>Subject/marks line returned as part of a saved exam result.</summary>
    public class ExamDtlResultDto
    {
        public int DtlsID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int Marks { get; set; }
    }

    /// <summary>Full saved exam record (ExamMaster + ExamDtls + Student) shown in the results table.</summary>
    public class ExamResultDto
    {
        public int MasterID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public int ExamYear { get; set; }
        public int TotalMark { get; set; }
        public string PassOrFail { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public List<ExamDtlResultDto> Subjects { get; set; } = new();
    }

    /// <summary>Payload sent when editing an existing exam record - only marks can change.</summary>
    public class UpdateExamRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one subject and mark must be provided.")]
        public List<ExamDtlRequestDto> Subjects { get; set; } = new();
    }
}
