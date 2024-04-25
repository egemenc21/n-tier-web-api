using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;

namespace Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController<UserService, AppUser, UserDto, UserDbEntryDto >
{

    public UserController(UserService userService, IMapper mapper) : base(userService, mapper)
    {

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] UserRegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (registerDto.File.Length == 0)
                return BadRequest("File is required.");
            
            
            var fileUrl = await _service.WriteFile(registerDto.File);

            UserDbEntryDto newUserRegistration = new()
            {
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                Email = registerDto.Email,
                Password = registerDto.Password,
                PhoneNumber = registerDto.PhoneNumber,
                ProfilePictureUrl = fileUrl,
            };
           
            var response = await _service.Register(newUserRegistration);

            if (response.Succeeded == false)
            {
                return StatusCode(500, response.Message);
            }

            return Ok(response);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(UserLoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _service.Login(request.Email, request.Password);

        if (user == null)
            return Unauthorized("Invalid email or password");

        return Ok(user);
    }

    [HttpPost("FileUpload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var fileUrl = await _service.WriteFile(file);
        return Ok(fileUrl);
    }


    
    
    
}