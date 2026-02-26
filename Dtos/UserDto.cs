namespace BluClinicsApi.Dtos
{
    public class GetUserByIdDto
    {
        public required int UserId { get; set; }
    }

    public class InActiveUserDto
    {
        public required int UserId { get; set; }
        public required string status { get; set; }
    }
}