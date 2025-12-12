
namespace API.src.models.viewModels
{
public class EmployeeViewModel
    {
        public int BusinessEntityID { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string JobTitle { get; set; }
        public required string Department { get; set; }
        public required string Email { get; set; }
        public DateOnly HireDate { get; set; }
    }
}