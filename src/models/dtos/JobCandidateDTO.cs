namespace API.src.models.dtos
{
    public class JobCandidateDTO
    {
        public int? JobCandidateId { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public string? Resume { get; set; }
        public required string Message { get; set; }
    }
}