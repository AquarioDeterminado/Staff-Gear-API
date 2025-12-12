namespace API.src.models.dtos
{
    public class EmployeeDTO
    {
        public int BusinessEntityID { get; set; }
        public string JobTitle { get; set; } = null!;
        public DateOnly HireDate { get; set; }
    }
}
