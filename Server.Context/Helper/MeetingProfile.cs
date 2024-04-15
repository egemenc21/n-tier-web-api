using AutoMapper;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.Context.Helper;

public class MeetingProfile : Profile
{
    public MeetingProfile()
    {
        CreateMap<Meeting, MeetingDto>();
        CreateMap<MeetingDto, Meeting>();
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

    }
}