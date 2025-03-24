namespace BlogCMSBackend.DTOs
{
    public class UserDto
    {
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        // Enhanced signup fields:
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
    }
}
