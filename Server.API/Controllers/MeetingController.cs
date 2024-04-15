using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly MeetingService _meetingService;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public MeetingController(MeetingService meetingService, UserService userService, IMapper mapper)
        {
            _meetingService = meetingService;
            _mapper = mapper;
            _userService = userService;
        }
        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Meeting>))]
        public async Task<ActionResult> GetAllMeetings()
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetings = _mapper.Map<List<MeetingDto>>(await _meetingService.GetAllMeetings());

            return Ok(meetings);
        }
        
        
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Meeting))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Meeting>> GetMeetingById(int id)
        {
            if (!_meetingService.MeetingExists(id))
            {
                return NotFound();
            }
            
            var meeting = _mapper.Map<MeetingDto>(await _meetingService.GetMeetingById(id)) ;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(meeting);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateMeeting([FromQuery] int userId, [FromBody] MeetingDto? meetingCreate)
        {
            if (meetingCreate == null)
                return BadRequest(ModelState);

            var meeting = await _meetingService.GetMeetingByName(meetingCreate.Name);

            if (meeting != null)
            {
                ModelState.AddModelError("", "Meeting already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var meetingMap = _mapper.Map<Meeting>(meetingCreate);
            meetingMap.User = await _userService.GetUserById(userId);

            if (!await _meetingService.CreateMeeting(meetingMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            
            return Ok("Successfully created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeeting(int id, Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return BadRequest();
            }

            await _meetingService.UpdateMeeting(id, meeting);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            await _meetingService.DeleteMeeting(id);
            return NoContent();
        }
    
    }
}
