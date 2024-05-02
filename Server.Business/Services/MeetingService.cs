using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Server.Context.Abstract;
using Server.Core.Email;
using Server.Model.Dtos;
using Server.Model.Dtos.Meeting;
using Server.Model.Models;

namespace Server.Business.Services;

public class MeetingService : IBaseService<Meeting, MeetingDto, MeetingDbEntryDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;

    public MeetingService(IMeetingRepository meetingRepository, IConfiguration configuration, IEmailSender emailSender,
        IUserRepository userRepository)
    {
        _meetingRepository = meetingRepository;
        _userRepository = userRepository;
        _configuration = configuration;
        _emailSender = emailSender;
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
        if (meeting.UserId == null) return false;

        var user = await _userRepository.GetUserByIdAsync(meeting.UserId);

        if (user == null) return false;

        var senderEmail = _configuration["SENDER_EMAIL"]!;
        var senderPassword = _configuration["SENDER_PASSWORD"]!;

        var createdMeeting = await _meetingRepository.CreateMeetingAsync(meeting);

        if (user.Email != null && createdMeeting != null)
        {
            var messageToSend =
                $" Your {meeting.Name} meeting is about {meeting.Description} and will start at {meeting.StartDate}, ends at {meeting.EndDate}";


            await _emailSender.SendEmailAsync(
                senderEmail,
                senderPassword,
                user.Email,
                "Your new meeting!",
                messageToSend
            );
            return true;
        }

        return false;
    }

    public async Task<bool> Update(string id, MeetingDto meetingToBeUpdated)
    {
        var existingMeeting = await _meetingRepository.GetByIdAsync(int.Parse(id));

        if (existingMeeting == null)
        {
            throw new KeyNotFoundException("Meeting not found");
        }

        if (meetingToBeUpdated.Document == null)
        {
            existingMeeting.Id = meetingToBeUpdated.Id;
            existingMeeting.Name = meetingToBeUpdated.Name;
            existingMeeting.StartDate = meetingToBeUpdated.StartDate;
            existingMeeting.EndDate = meetingToBeUpdated.EndDate;
            existingMeeting.Description = meetingToBeUpdated.Description;
        }
        else
        {
            var newDocumentUrl = await WriteFile(meetingToBeUpdated.Document);
            existingMeeting.Id = meetingToBeUpdated.Id;
            existingMeeting.Name = meetingToBeUpdated.Name;
            existingMeeting.StartDate = meetingToBeUpdated.StartDate;
            existingMeeting.EndDate = meetingToBeUpdated.EndDate;
            existingMeeting.Description = meetingToBeUpdated.Description;
            existingMeeting.DocumentUrl = newDocumentUrl;
        }

        if (int.Parse(id) != meetingToBeUpdated.Id)
        {
            return false;
        }

        return await _meetingRepository.UpdateAsync(existingMeeting);
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