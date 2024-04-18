using Microsoft.AspNetCore.Identity;

namespace Server.Model.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
   
    public string ProfilePictureUrl { get; set; } = string.Empty;
    
    //relational properties
    public virtual ICollection<Meeting>? Meetings { get; set; }
}