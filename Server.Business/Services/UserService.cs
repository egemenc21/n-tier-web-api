using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Context.Abstract;
using Server.Core.Email;
using Server.Model.Dtos;
using Server.Model.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Server.Business.Services;

public class UserService : IBaseService<User>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;

    public UserService(IConfiguration configuration, IEmailSender emailSender, IUserRepository userRepository)
    {
        _configuration = configuration;
        _emailSender = emailSender;
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAll()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<User> GetById(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<bool> Create(User entity)
    {
        if (_userRepository.UserExists(entity.Id))
        {
            return false;
        }

        await _userRepository.AddAsync(entity);
        
        return true;
    }

    public async Task<bool> Update(int id, User user)
    {
        if (id != user.Id)
        {
            return false;
        }
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }


    public async Task<User> Register(UserRegisterDto request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User()
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            HashedPassword = hashedPassword,
            PhoneNumber = request.PhoneNumber,
            ProfilePictureUrl = request.ProfilePictureUrl
        };

        await Create(user);

        var senderEmail = _configuration["SENDER_EMAIL"]!;
        var senderPassword = _configuration["SENDER_PASSWORD"]!;

        await _emailSender.SendEmailAsync(senderEmail, senderPassword, request.Email, "dotnet",
            "Welcome to our app!");

        return user;
    }

    public async Task<string> Login(UserLoginDto request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = GenerateToken(user);

        return token;
    }

    public async Task Delete(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public bool Exists(int id)
    {
        return _userRepository.UserExists(id);
    }

    private string GenerateToken(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}