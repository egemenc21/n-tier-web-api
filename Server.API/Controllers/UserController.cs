using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController<UserService, User, UserDto>
    {
        public UserController(UserService service, IMapper mapper) : base(service, mapper)
        {
    
        }

        // [HttpGet]
        // [ProducesResponseType(200, Type = typeof(List<User>))]
        // public async Task<IActionResult> GetUsers()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //
        //     var users = _mapper.Map<List<UserDto>>(await (_userService).GetAll());
        //
        //     return Ok(users);
        // }
        
        // [HttpGet("{userId}")]
        // [ProducesResponseType(200, Type = typeof(User))]
        // [ProducesResponseType(400)]
        // public async Task<ActionResult<Meeting>> GetUserById(int userId)
        // {
        //     if (!_userService.Exists(userId))
        //     {
        //         return NotFound();
        //     }
        //     
        //     var meeting = _mapper.Map<UserDto>(await _userService.GetById(userId)) ;
        //
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     
        //     return Ok(meeting);
        // } 

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var user = await _service.Register(request);

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto request)
        {
            var token = await _service.Login(request);

            return Ok(token);
        }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(int id)
        // {
        //     try
        //     {
        //         await _userService.Delete(id);
        //         return Ok("User Deleted Successfully");
        //     }
        //     catch (KeyNotFoundException)
        //     {
        //         return NotFound();
        //     }
        // }
        
        
    }
}