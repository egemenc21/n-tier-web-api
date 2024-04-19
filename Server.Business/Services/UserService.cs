using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Server.Business.Token;
using Server.Context.Abstract;
using Server.Core.Email;
using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;

namespace Server.Business.Services;

public class UserService : IBaseService<AppUser>
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

    // public async Task<bool> Create(AppUser entity)
    // {
    //     if (_userRepository.UserExists(entity.Id))
    //     {
    //         return false;
    //     }
    //
    //     await _userRepository.AddAsync(entity);
    //     
    //     return true;
    // }

    public async Task<bool> Update(string id, AppUser user)
    {
        if (id != user.Id)
        {
            return false;
        }
        
        var data = await _userRepository.UpdateAsync(user);

        return data;
    }

    public async Task<AppUser?> GetUserByEmail(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }


    public async Task<CreateUserResponse> Register(UserRegisterDto request)
    {
        var response = await _userRepository.AddAsync(request);

        var senderEmail = _configuration["SENDER_EMAIL"]!;
        var senderPassword = _configuration["SENDER_PASSWORD"]!;

        await _emailSender.SendEmailAsync(senderEmail, senderPassword, request.Email, "dotnet",
            "Welcome to our app!");

        return response;
    }

    public async Task<NewUserDto?> Login(string email, string password)
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

        return new NewUserDto
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
}