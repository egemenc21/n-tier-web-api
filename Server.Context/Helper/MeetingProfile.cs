using AutoMapper;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.Context.Helper;

public class MeetingProfile : Profile
{
    public MeetingProfile()
    {
        CreateMap<Meeting, MeetingDto>();
        CreateMap<User, UserDto>();
        CreateMap<MeetingDto, Meeting>();

    }
}