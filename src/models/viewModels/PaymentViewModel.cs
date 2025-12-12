namespace API.src.models.viewModels
{
    class PaymentViewModel
    {
        public int BusinessEntityID { get; set; }
        public string FullName { get; set; }
        public decimal Rate { get; set; }
        public DateTime PayedDate { get; set; }
    }
}