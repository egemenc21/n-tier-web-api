namespace Server.Model.Dtos;

public class UserRegisterDto
{
    public required string Name { get; set; } = string.Empty;
    public required string Surname { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;

    public required string PhoneNumber { get; set; } = string.Empty;
    public required string ProfilePictureUrl { get; set; } = string.Empty;
}