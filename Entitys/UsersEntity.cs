using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BluClinicsApi.Entitys
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string? UhId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Mobile { get; set; }
        public required string Password { get; set; }
        public required int RoleId { get; set; }
        public string? ImageUrl { get; set; }
        public string IsActive { get; set; } = "Active";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}