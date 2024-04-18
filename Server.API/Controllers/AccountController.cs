using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Business.Services;
using Server.Business.Token;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly UserService _userService;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager, UserService userService)
    {
        _userManager = userManager;

        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // var appuser = new AppUser()
            // {
            //     UserName = registerDto.Email,
            //     Email = registerDto.Email,
            //     PhoneNumber = registerDto.PhoneNumber
            // };
            //
            // var createdUser = await _userManager.CreateAsync(appuser, registerDto.Password);

            //     if (createdUser.Succeeded)
            //     {
            //         var roleResult = await _userManager.AddToRoleAsync(appuser, "User");
            //         if (roleResult.Succeeded)
            //         {
            //             var token = _tokenService.GenerateToken(appuser);
            //             var data = new NewUserDto()
            //             {
            //                 Id = appuser.Id,
            //                 Email = appuser.Email,
            //                 Token = token
            //             };
            //             return Ok(data);
            //         }
            //         else
            //         {
            //             return StatusCode(500, roleResult.Errors);
            //         }
            //     }
            //     else
            //     {
            //         return StatusCode(500, createdUser.Errors);
            //     }
            // }

            var response = await _userService.Register(registerDto);
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