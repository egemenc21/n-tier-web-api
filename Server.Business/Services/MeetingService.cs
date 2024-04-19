using Server.Context.Abstract;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.Business.Services;

public class MeetingService : IBaseService<Meeting>
{
    private readonly IMeetingRepository _meetingRepository;

    public MeetingService(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<List<Meeting>> GetAll()
    {
        return await _meetingRepository.GetMeetings();
    }

    public async Task<List<Meeting?>> GetMeetingsByUserId(string userId)
    {
        return await _meetingRepository.GetMeetingsByUserId(userId);
    }

    public async Task<Meeting?> GetById(string id)
    {
        return await _meetingRepository.GetByIdAsync(int.Parse(id));
    }
    
    public async Task<Meeting?> GetMeetingByName(string name)
    {
        return await _meetingRepository.GetMeetingByNameAsync(name);
    }

    public async Task<bool> Create(Meeting meeting)
    {
        return await _meetingRepository.CreateMeetingAsync(meeting);
    }

    public async Task<bool> Update(string id, Meeting meeting)
    {
        var existingMeeting = await _meetingRepository.GetByIdAsync(int.Parse(id));
        
        if (existingMeeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }
        
        if (int.Parse(id) != meeting.Id)
        {
            return false;
        }

        _meetingRepository.Detach(existingMeeting);

        return await _meetingRepository.UpdateAsync(meeting);;
    }

    public async Task Delete(string id)
    {
        await _meetingRepository.DeleteAsync(int.Parse(id));
    }

    public async Task<bool>  Exists(string id)
    {
        return await _meetingRepository.MeetingExists(int.Parse(id));
    }
}