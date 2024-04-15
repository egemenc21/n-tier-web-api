using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model.Models;

public class Meeting : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    
    
    public int? UserId { get; set; } 
    
    //relational properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}