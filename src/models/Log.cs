using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.src.models;

public class Log 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogID { get; set; }

    public int? ActorID { get; set; }

    public string? Target { get; set; } 

    public string? Action { get; set; }

    public DateTime CreatedAt { get; set; }
}