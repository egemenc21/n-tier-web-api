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

    public async Task<List<Meeting?>> GetMeetingsByUserId(int userId)
    {
        return await _meetingRepository.GetMeetingsByUserId(userId);
    }

    public async Task<Meeting> GetById(int id)
    {
        return await _meetingRepository.GetByIdAsync(id);
    }
    
    public async Task<Meeting?> GetMeetingByName(string name)
    {
        return await _meetingRepository.GetMeetingByNameAsync(name);
    }

    public async Task<bool> Create(Meeting meeting)
    {
        return await _meetingRepository.CreateMeetingAsync(meeting);
    }

    public async Task<bool> Update(int id, Meeting meeting)
    {
        var existingMeeting = await _meetingRepository.GetByIdAsync(id);
        
        if (existingMeeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }
        
        if (id != meeting.Id)
        {
            return false;
        }

        _meetingRepository.Detach(existingMeeting);

        return await _meetingRepository.UpdateAsync(meeting);;
    }

    public async Task Delete(int id)
    {
        await _meetingRepository.DeleteAsync(id);
    }

    public bool Exists(int id)
    {
        return _meetingRepository.MeetingExists(id);
    }
}