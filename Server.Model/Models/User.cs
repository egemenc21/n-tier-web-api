namespace Server.Model.Models;

public class User : BaseModel
{
    
    public string Name { get; set; } = string.Empty; 
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    
    //relational properties
    public virtual List<Meeting>? Meetings { get; set; }

}