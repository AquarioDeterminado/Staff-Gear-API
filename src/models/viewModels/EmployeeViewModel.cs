
namespace API.src.models.viewModels
{
public class EmployeeViewModel
    {
        public int? BusinessEntityID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string?  Email { get; set; }
        public DateOnly HireDate { get; set; }
    }
}