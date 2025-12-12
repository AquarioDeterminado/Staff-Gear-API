namespace API.src.models.viewModels
{
    class MovementViewModel
    {
        public int BusinessEntityID { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}