import { create } from 'zustand';
import type { ChatRoom, Message, FileTransfer } from '../types/types';

interface RoomCache {
  messages: Message[];
  files: FileTransfer[];
}

interface ChatState {
  rooms: ChatRoom[];
  activeRoom: ChatRoom | null;
  messages: Message[];
  files: FileTransfer[];
  typingUsers: string[];
  _cache: Record<number, RoomCache>;

  setRooms: (rooms: ChatRoom[]) => void;
  addRoom: (room: ChatRoom) => void;
  setActiveRoom: (room: ChatRoom | null) => void;
  setMessages: (messages: Message[]) => void;
  addMessage: (message: Message) => void;
  setFiles: (files: FileTransfer[]) => void;
  addFile: (file: FileTransfer) => void;
  setTyping: (identifier: string, isTyping: boolean) => void;
}

export const useChatStore = create<ChatState>((set, get) => ({
  rooms: [],
  activeRoom: null,
  messages: [],
  files: [],
  typingUsers: [],
  _cache: {},

  setRooms: (rooms) => set({ rooms }),

  addRoom: (room) =>
    set((s) => ({
      rooms: s.rooms.find((r) => r.id === room.id) ? s.rooms : [...s.rooms, room],
    })),

  setActiveRoom: (newRoom) =>
    set((s) => {
      const cache = { ...s._cache };

      if (s.activeRoom) {
        cache[s.activeRoom.id] = { messages: s.messages, files: s.files };
      }

      const cached = newRoom ? cache[newRoom.id] : undefined;

      return {
        activeRoom: newRoom,
        messages: cached?.messages ?? [],
        files: cached?.files ?? [],
        typingUsers: [],
        _cache: cache,
      };
    }),

  setMessages: (messages) => set({ messages }),

  addMessage: (message) =>
    set((s) => {
      const isDup = s.messages.some(
        (m) =>
          m.content === message.content &&
          m.sender?.username === message.sender?.username &&
          Math.abs(new Date(m.sentAt).getTime() - new Date(message.sentAt).getTime()) < 10000
      );
      if (isDup) return {};
      return { messages: [...s.messages, message] };
    }),

  setFiles: (files) => set({ files }),

  addFile: (file) =>
    set((s) => {
      const isDup = s.files.some(
        (f) => f.nomeOriginal === file.nomeOriginal && f.roomId === file.roomId && f.tamanhoBytes === file.tamanhoBytes
      );
      if (isDup) return {};
      return { files: [...s.files, file] };
    }),

  setTyping: (identifier, isTyping) =>
    set((s) => ({
      typingUsers: isTyping
        ? [...s.typingUsers.filter((u) => u !== identifier), identifier]
        : s.typingUsers.filter((u) => u !== identifier),
    })),
}));
