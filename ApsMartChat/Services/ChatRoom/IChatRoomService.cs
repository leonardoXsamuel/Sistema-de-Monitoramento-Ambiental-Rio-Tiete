using ApsMartChat.DTOs.ChatRoom;

namespace ApsMartChat.Services.ChatRoom;

public interface IChatRoomService
{
    public Task<ChatRoomResponseDTO> AlterarNomeChatRoom(int roomId, ChatRoomUpdateDTO chatRoomUpdateDTO);

    public Task<ChatRoomResponseDTO> CriarChatRoomAsync(ChatRoomCreateDTO dto);
}
