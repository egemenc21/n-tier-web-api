using Microsoft.AspNetCore.Http;
using Server.Context.Abstract;
using Server.Model.Dtos;
using Server.Model.Dtos.Meeting;
using Server.Model.Models;

namespace Server.Business.Services;

public class MeetingService : IBaseService<Meeting, MeetingDbEntryDto>
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

    public async Task<bool> Update(string id, MeetingDbEntryDto meetingDbEntryDto)
    {
        var existingMeeting = await _meetingRepository.GetByIdAsync(int.Parse(id));

        if (existingMeeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }

        if (int.Parse(id) != meetingDbEntryDto.Id)
        {
            return false;
        }

        existingMeeting.Id = meetingDbEntryDto.Id;
        existingMeeting.Name = meetingDbEntryDto.Name;
        existingMeeting.StartDate = meetingDbEntryDto.StartDate;
        existingMeeting.EndDate = meetingDbEntryDto.EndDate;
        existingMeeting.Description = meetingDbEntryDto.Description;
        existingMeeting.DocumentUrl = meetingDbEntryDto.DocumentUrl;

        return await _meetingRepository.UpdateAsync(existingMeeting);
        ;
    }

    public async Task Delete(string id)
    {
        var meetingToBeDeleted = await _meetingRepository.GetByIdAsync(int.Parse(id));
        var pathToBeDeleted = meetingToBeDeleted?.DocumentUrl;
        if (pathToBeDeleted != null) await DeleteFile(pathToBeDeleted);

        await _meetingRepository.DeleteAsync(int.Parse(id));
    }

    public async Task<bool> Exists(string id)
    {
        return await _meetingRepository.MeetingExists(int.Parse(id));
    }

    public async Task<string> WriteFile(IFormFile file)
    {
        // Generate a unique file name
        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Documents");

        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

        var exactPath = Path.Combine(filePath, uniqueFileName);

        // Save the file to the local directory
        await using var stream = File.Create(exactPath);
        await file.CopyToAsync(stream);

        return uniqueFileName;
    }

    public Task DeleteFile(string path)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Documents", path);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}