namespace ApsMartChat.Profiles;

using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.Models;
using AutoMapper;

public class ChatRoomProfile : Profile
{
    public ChatRoomProfile()
    {
        CreateMap<ChatRoomCreateDTO, ChatRoom>();
        CreateMap<ChatRoomUpdateDTO, ChatRoom>();
        CreateMap<ChatRoom, ChatRoomResponseDTO>();
    }

}