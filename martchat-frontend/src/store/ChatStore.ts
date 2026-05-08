import { create } from "zustand";
import type {
  MessageResponseDTO,
  FileTransferResponseDTO,
  RoomWithId,
} from "../types/Types";

interface ChatState {
  rooms: RoomWithId[];
  currentRoomId: number | null;
  messages: MessageResponseDTO[];
  files: FileTransferResponseDTO[];
  typingUsers: string[];
  connected: boolean;

  setRooms: (rooms: RoomWithId[]) => void;
  addRoom: (room: RoomWithId) => void;
  setCurrentRoom: (id: number | null) => void;
  setMessages: (msgs: MessageResponseDTO[]) => void;
  addMessage: (msg: MessageResponseDTO) => void;
  setFiles: (files: FileTransferResponseDTO[]) => void;
  addFile: (file: FileTransferResponseDTO) => void;
  setTypingUser: (username: string, isTyping: boolean) => void;
  setConnected: (v: boolean) => void;
}

export const useChatStore = create<ChatState>((set) => ({
  rooms: [],
  currentRoomId: null,
  messages: [],
  files: [],
  typingUsers: [],
  connected: false,

  setRooms: (rooms) => set({ rooms }),
  addRoom: (room) => set((s) => ({ rooms: [...s.rooms, room] })),
  setCurrentRoom: (id) =>
  set(() => ({
    currentRoomId: id,
    messages: [],
    files: [],
  })),
  setMessages: (msgs) => set({ messages: msgs }),
  addMessage: (msg) =>
    set((state) => {
      // filtra por sala
      if (msg.room?.id !== state.currentRoomId) return state;

      // evita duplicado
      const exists = state.messages.some(
        (m) =>
          m.content === msg.content &&
          m.sentAt === msg.sentAt &&
          m.sender?.username === msg.sender?.username,
      );

      if (exists) return state;

      return {
        messages: [...state.messages, msg],
      };
    }),
  setFiles: (files) => set({ files }),
  addFile: (file) => set((s) => ({ files: [...s.files, file] })),
  setConnected: (v) => set({ connected: v }),

  setTypingUser: (username, isTyping) =>
    set((s) => ({
      typingUsers: isTyping
        ? [...new Set([...s.typingUsers, username])]
        : s.typingUsers.filter((u) => u !== username),
    })),
}));
