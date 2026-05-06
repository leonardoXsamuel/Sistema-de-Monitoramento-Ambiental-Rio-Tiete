//   Auth  
export interface LoginRequest {
  username: string
  password: string
}
 
export interface RegisterRequest {
  username: string
  password: string
  displayName: string
  role: 'Inspetor' | 'Coordenador'
}
 
export interface AuthResponse {
  token: string
  username: string
  displayName: string
  role: 'Inspetor' | 'Coordenador'
}
 
//   ChatRoom                                  
export interface ChatRoomResponseDTO {
  name: string
  createdAt: string
  messages: MessageResponseDTO[]
  fileTransfers: FileTransferResponseDTO[]
  id?: number // helper frontend
}
 
export interface ChatRoomCreateDTO {
  name: string
}
 
//   Message                                 
export interface MessageResponseDTO {
  content: string
  sentAt: string
  sender: UserResponseDTO
  room?: ChatRoomResponseDTO
}

//   FileTransfer                                
export interface FileTransferResponseDTO {
  nomeOriginal: string
  tipoConteudo: string
  tamanhoBytes: number
  uploadedAt: string
  downloadUrl: string
  uploaderId?: number
  roomId?: number
}
 
//   User  
export interface UserResponseDTO {
  username: string
  displayName: string
  role: string
  createdAt: string
}
 
//   UI helpers                                 
export interface RoomWithId {
  id: number
  name: string
  createdAt: string
}