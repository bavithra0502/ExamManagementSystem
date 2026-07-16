namespace ExamManagementAPI.Helpers
{
    /// <summary>Thrown when a requested entity does not exist. Mapped to HTTP 404.</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>Thrown when input fails a business rule. Mapped to HTTP 400.</summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>Thrown when an operation would violate a uniqueness rule. Mapped to HTTP 409.</summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
