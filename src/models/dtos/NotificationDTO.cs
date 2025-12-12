namespace API.src.models.dtos
{
    public class NotificationDTO
    {
        public int NotificationID { get; set; }

        public required string Message { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int BusinessEntityID { get; set; }
    }
}