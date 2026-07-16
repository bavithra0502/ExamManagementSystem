namespace ExamManagementAPI.DTOs
{
    public class StudentDto
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
    }

    public class StudentCreateDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
    }
}
