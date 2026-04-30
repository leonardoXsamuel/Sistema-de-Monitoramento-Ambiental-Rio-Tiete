namespace ApsMartChat.Profiles;

using AutoMapper;
using ApsMartChat.Models;
using ApsMartChat.DTOs.User;
using ApsMartChat.DTOs.ChatRoom;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDTO, User>();
        CreateMap<UserUpdateDTO, User>();
        CreateMap<User, UserResponseDTO>();
    }

}