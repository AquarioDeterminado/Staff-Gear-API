namespace API.src.models.dtos
{
    public class EmployeeDTO
    {
        public int BusinessEntityID { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly HireDate { get; set; }
    }
}
