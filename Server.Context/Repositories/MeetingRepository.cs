using Microsoft.EntityFrameworkCore;
using Server.Context.Abstract;
using Server.Context.Context;
using Server.Model.Models;

namespace Server.Context.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly ApplicationDbContext _context;

    public MeetingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Meeting?>> GetMeetings()
    {
        return await _context.Meetings.ToListAsync();
    }

    public async Task<Meeting?> GetMeetingByNameAsync(string name)
    {
        return await _context.Meetings.Where(m => m.Name.Trim().ToUpper() == name.TrimEnd().ToUpper())
            .FirstOrDefaultAsync();
    }

    public async Task<Meeting> GetByIdAsync(int id)
    {
        return await _context.Meetings.Where(m => m.Id == id).FirstOrDefaultAsync() ??
               throw new InvalidOperationException();
    }

    public async Task<bool> CreateMeetingAsync(Meeting? meeting)
    {
        _context.Meetings.Add(meeting);
        return await Save();
    }

    public async Task<Meeting> UpdateAsync(Meeting meeting)
    {
        _context.Entry(meeting).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return meeting;
    }

    public async Task DeleteAsync(int id)
    {
        var meeting = await _context.Meetings.FindAsync(id);
        if (meeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }

        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync();
    }

    public bool MeetingExists(int id)
    {
        return _context.Meetings.Any(m => m.Id == id);
    }

    public async Task<bool> Save()
    {
        var saved = await _context.SaveChangesAsync();

        return saved > 0 ? true : false;
    }
}