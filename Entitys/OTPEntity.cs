using System.ComponentModel.DataAnnotations;

namespace BluClinicsApi.Entitys
{
    public class OTPS
    {
        [Key]
        public int OtpId { get; set; }
        public required string Type { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public required string Otp { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpireDate { get; set; }
    }
}