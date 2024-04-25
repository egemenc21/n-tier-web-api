using AutoMapper;
using Server.Model.Dtos;
using Server.Model.Dtos.Meeting;
using Server.Model.Models;

namespace Server.Context.Helper;

public class MeetingProfile : Profile
{
    public MeetingProfile()
    {
        CreateMap<Meeting, MeetingDbEntryDto>();
        CreateMap<MeetingDbEntryDto, Meeting>();
        CreateMap<AppUser, UserDto>();
        CreateMap<UserDto, AppUser>();
        
    }
}