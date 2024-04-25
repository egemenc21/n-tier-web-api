using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Server.Business.Token;
using Server.Context.Abstract;
using Server.Core.Email;
using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;

namespace Server.Business.Services;

public class UserService : IBaseService<AppUser, UserDto, UserDbEntryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public UserService(IConfiguration configuration, IEmailSender emailSender, IUserRepository userRepository,
        SignInManager<AppUser> signInManager, ITokenService tokenService)
    {
        _configuration = configuration;
        _emailSender = emailSender;
        _userRepository = userRepository;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }
    

    public async Task<List<AppUser>> GetAll()
    {
        return await _userRepository.GetUsersAsync();
    }
    
    public async Task<AppUser?> GetById(string id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }
    

    public async Task<bool> Update(string id, UserDto user)
    {
        if (id != user.Id)
        {
            return false;
        }
        
        var existingUser = await _userRepository.GetUserByIdAsync(id);
     

        existingUser!.Id = user.Id;
        existingUser!.UserName = user.UserName;
        existingUser!.Name = user.Name;
        existingUser!.Surname = user.Surname;
        existingUser!.Email = user.Email;
        existingUser!.PasswordHash = user.PasswordHash;
        existingUser!.PhoneNumber = user.PhoneNumber;
        existingUser!.ProfilePictureUrl = user.ProfilePictureUrl;
        
        
        
        
        var data = await _userRepository.UpdateAsync(existingUser);

        return data;
    }

    public async Task<AppUser?> GetUserByEmail(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }


    public async Task<CreateUserResponse> Register(UserDbEntryDto request)
    {
        var response = await _userRepository.AddAsync(request);

        var senderEmail = _configuration["SENDER_EMAIL"]!;
        var senderPassword = _configuration["SENDER_PASSWORD"]!;

        await _emailSender.SendEmailAsync(senderEmail, senderPassword, request.Email, "dotnet",
            "Welcome to our app!");

        return response;
    }

    public async Task<JwtUserDto?> Login(string email, string password)
    {
        var user = await GetUserByEmail(email);

        if (user == null)
        {
            return null; // Or throw an exception, depending on your requirements
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
        {
            return null; // Or throw an exception, depending on your requirements
        }

        return new JwtUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Token = _tokenService.GenerateToken(user)
        };
        ;
    }

    public async Task Delete(string id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public async Task<bool> Exists(string id)
    {
        return await _userRepository.UserExists(id);
    }
    
    public async Task<string> WriteFile(IFormFile file)
    {
        // Generate a unique file name
        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Files");

        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

        var exactPath = Path.Combine(filePath, uniqueFileName);
            
        // Save the file to the local directory
        await using (var stream = System.IO.File.Create(exactPath))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }
}