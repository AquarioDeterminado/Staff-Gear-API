namespace API.src.models.dtos
{
    public class EmployeePayHistoryDTO
    {
        public int BusinessEntityID { get; set; }
        public DateTime RateChangeDate { get; set; }
        public decimal Rate { get; set; }
        public byte PayFrequency { get; set; }
        public DateTime ModifiedDate { get; set; }
        // Assuming there's a field for tracking who modified the record,
        // given that there is a virtual employee on the main object?
        public Guid ModifiedBy { get; set; }
    }
}