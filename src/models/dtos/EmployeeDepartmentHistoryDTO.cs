namespace API.src.models.dtos
{
    public class EmployeeDepartmentHistoryDTO
    {
        public int? BusinessEntityID { get; set; }
        public int DepartmentID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}