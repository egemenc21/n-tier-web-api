using Server.Model.Models;

namespace Server.Context.Abstract;

public interface IMeetingRepository
{
    Task<Meeting?> GetByIdAsync(int id);
    Task<List<Meeting>> GetMeetings();

    Task<List<Meeting?>> GetMeetingsByUserId(string userId);
    
    Task<Meeting?> GetMeetingByNameAsync(string name);
    Task<bool> CreateMeetingAsync(Meeting meeting);
    Task<bool> UpdateAsync(Meeting meeting);
    Task DeleteAsync(int id);

    Task<bool>  MeetingExists(int id);

    Task<bool> Save();

    void Detach(Meeting meeting);
}