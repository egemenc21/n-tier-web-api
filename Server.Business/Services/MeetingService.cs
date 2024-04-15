using Server.Context.Abstract;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.Business.Services;

public class MeetingService
{
    private readonly IMeetingRepository _meetingRepository;

    public MeetingService(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<List<Meeting?>> GetAllMeetings()
    {
        return await _meetingRepository.GetMeetings();
    }

    public async Task<Meeting> GetMeetingById(int id)
    {
        return await _meetingRepository.GetByIdAsync(id);
    }
    
    public async Task<Meeting?> GetMeetingByName(string name)
    {
        return await _meetingRepository.GetMeetingByNameAsync(name);
    }

    public async Task<bool> CreateMeeting(Meeting meeting)
    {
        return await _meetingRepository.CreateMeetingAsync(meeting);
    }

    public async Task<Meeting> UpdateMeeting(int id, Meeting meeting)
    {
        var existingMeeting = await _meetingRepository.GetByIdAsync(id);
        if (existingMeeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }

        // Update existingMeeting properties with meeting properties
        existingMeeting.Name = meeting.Name;
        existingMeeting.StartDate = meeting.StartDate;
        existingMeeting.EndDate = meeting.EndDate;
        existingMeeting.Description = meeting.Description;
        existingMeeting.DocumentUrl = meeting.DocumentUrl;
        existingMeeting.UserId = meeting.UserId;

        return await _meetingRepository.UpdateAsync(existingMeeting);
    }

    public async Task DeleteMeeting(int id)
    {
        await _meetingRepository.DeleteAsync(id);
    }

    public bool MeetingExists(int id)
    {
        return _meetingRepository.MeetingExists(id);
    }
}