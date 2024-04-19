using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController<UserService, AppUser, UserDto >
{
    private readonly UserService _userService;

    public UserController(UserService userService, IMapper mapper) : base(userService, mapper)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userService.Register(registerDto);

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

        var user = await _userService.Login(request.Email, request.Password);

        if (user == null)
            return Unauthorized("Invalid email or password");

        return Ok(user);
    }
    
}