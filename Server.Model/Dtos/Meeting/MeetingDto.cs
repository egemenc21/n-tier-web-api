using Microsoft.AspNetCore.Http;

namespace Server.Model.Dtos.Meeting;

public class MeetingDto
{
    public int Id { set; get; }
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Description { get; set; }
    public IFormFile Document { get; set; }
}