namespace APIO.src.models.dtos
{
    public class PassWordDTO
    {
        /// <summary>
        /// Primary key for Employee records.
        /// </summary>
        public int BusinessEntityID { get; set; }

        /// <summary>
        /// Password for the e-mail account.
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Random value concatenated with the password string before the password is hashed.
        /// </summary>
        public string PasswordSalt { get; set; } = null!;
    }
}