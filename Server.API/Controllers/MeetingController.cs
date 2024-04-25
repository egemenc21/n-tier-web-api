using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Dtos.Meeting;
using Server.Model.Models;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : BaseController<MeetingService, Meeting, MeetingDbEntryDto>
    {
        public MeetingController(MeetingService service, IMapper mapper) : base(service, mapper)
        {
        }
        
        [HttpGet("User/{userId}")]
        [Authorize]
        public async Task<ActionResult> GetMeetingsByUserId(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetings = _mapper.Map<List<MeetingDbEntryDto>>(await _service.GetMeetingsByUserId(userId));

            return Ok(meetings);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMeeting([FromQuery] string userId, [FromForm] MeetingDto? meetingCreate)
        {
            if (meetingCreate == null)
                return BadRequest(ModelState);

            var meeting = await _service.GetMeetingByName(meetingCreate.Name);

            if (meeting != null)
            {
                ModelState.AddModelError("", "Meeting already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var documentUrl = await _service.WriteFile(meetingCreate.Document);

            Meeting newMeeting = new()
            {
                Name = meetingCreate.Name,
                StartDate = meetingCreate.StartDate,
                EndDate = meetingCreate.EndDate,
                Description = meetingCreate.Description,
                DocumentUrl = documentUrl,
                UserId = userId
            };
            //
            // var meetingMap = _mapper.Map<Meeting>(meetingCreate);
            // meetingMap.UserId = userId;

            if (!await _service.Create(newMeeting))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            
            return Ok(new { message = "Successfully created" });
        }
    }
}
