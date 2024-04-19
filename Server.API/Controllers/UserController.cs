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
    
    [HttpPut("{id}")]
    [Authorize]
    public override async Task<IActionResult> Update(string id, [FromBody] UserDto entityToBeUpdated)
    {
        if (!await _service.Exists(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // var entityMap = _mapper.Map<TEntity>(entityToBeUpdated);

        // Console.WriteLine(entityMap + "_________________________________________");
          
        var existingUser = await _userService.GetById(id);
     

        existingUser!.Id = entityToBeUpdated.Id;
        existingUser!.UserName = entityToBeUpdated.UserName;
        existingUser!.Name = entityToBeUpdated.Name;
        existingUser!.Surname = entityToBeUpdated.Surname;
        existingUser!.Email = entityToBeUpdated.PasswordHash;
        existingUser!.PhoneNumber = entityToBeUpdated.PhoneNumber;
        existingUser!.ProfilePictureUrl = entityToBeUpdated.ProfilePictureUrl;

        try
        {
            if (!await _service.Update(id, existingUser))
            {
                ModelState.AddModelError("", "Something went wrong updating category (id's are not matching)");
                return StatusCode(500, ModelState);
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}