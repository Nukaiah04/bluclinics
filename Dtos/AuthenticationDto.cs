namespace BluClinicsApi.Dtos
{
    public class RegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Mobile { get; set; }
        public required string Password { get; set; }
        public required int RoleId { get; set; }
        public string? ImageUrl { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class EmailLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        public required int UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }

    public class GenerateOtpDto
    {
        public required string Type { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
    }

    public class VerifyEmailOtpDto
    {
        public required string Type { get; set; }
        public required string Otp { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
    }
}