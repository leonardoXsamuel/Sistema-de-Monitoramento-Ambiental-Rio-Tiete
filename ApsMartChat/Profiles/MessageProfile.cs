namespace ApsMartChat.Profiles;

using AutoMapper;
using ApsMartChat.Models;
using ApsMartChat.DTOs.Message;
using ApsMartChat.DTOs.User;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<MessageCreateDTO, Message>();
        CreateMap<MessageUpdateDTO, Message>();
        CreateMap<Message, MessageResponseDTO>();
    }
}