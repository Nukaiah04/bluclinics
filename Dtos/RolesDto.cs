namespace BluClinicsApi.Dtos
{
    public class RoleGetByIdDto
    {
        public required int RoleId { get; set; }
    }
    public class RoleUpdateDto
    {
        public required int RoleId { get; set; }
        public required string IsActive { get; set; }
    }
}