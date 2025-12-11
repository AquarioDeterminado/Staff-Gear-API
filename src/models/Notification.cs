using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.src.models;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotificationID { get; set; }

    [MaxLength(300)]
    public required string Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public required int BusinessEntityID { get; set; }
}