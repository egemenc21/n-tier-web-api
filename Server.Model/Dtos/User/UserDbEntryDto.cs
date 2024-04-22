using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Server.Model.Dtos.User;

public class UserDbEntryDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Surname { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string ProfilePictureUrl { get; set; } = string.Empty;

}