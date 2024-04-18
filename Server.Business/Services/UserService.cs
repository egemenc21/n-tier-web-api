using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Business.Token;
using Server.Context.Abstract;
using Server.Core.Email;
using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Server.Business.Services;

public class UserService
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

        return await _userRepository.UpdateAsync(user);
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

    public async Task Delete(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public bool Exists(int id)
    {
        return _userRepository.UserExists(id);
    }
}