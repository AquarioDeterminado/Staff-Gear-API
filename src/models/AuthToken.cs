using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.src.models
{
    public partial class AuthToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthTokenID { get; set; }
        public int UserID { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}