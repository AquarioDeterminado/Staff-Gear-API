namespace API.src.models.dtos
{
    public class AuthTokenDTO
    {
        public int AuthTokenID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}