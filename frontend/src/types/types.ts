export type UserRole = 'Inspetor' | 'Coordenador';

export interface AuthUser {
  username: string;
  displayName: string;
  role: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  displayName: string;
  role: UserRole;
}

export interface AuthResponse {
  token: string;
  username: string;
  displayName: string;
  role: string;
}

export interface MessageSender {
  username: string;
  displayName: string;
  role: string;
  createdAt: string;
}

export interface Message {
  content: string;
  sentAt: string;
  sender: MessageSender;
}

export interface FileTransfer {
  nomeOriginal: string;
  tipoConteudo: string;
  tamanhoBytes: number;
  uploadedAt: string;
  downloadUrl: string;
  uploaderId: number | null;
  roomId: number | null;
}

export interface ChatRoom {
  id: number;
  name: string;
  createdAt: string;
  messages: Message[];
  fileTransfers: FileTransfer[];
}
