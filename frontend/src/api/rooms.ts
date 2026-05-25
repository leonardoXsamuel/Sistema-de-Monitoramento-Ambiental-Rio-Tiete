import http from './http';
import type { ChatRoom } from '../types/types';

export const getAllRooms = async (page = 1, pageSize = 50): Promise<ChatRoom[]> => {
  try {
    const res = await http.get<ChatRoom[]>('/api/chatroom/GetAllRooms', {
      params: { page, pageSize },
    });
    return res.data;
  } catch (err: any) {
    if (err.response?.status === 404) return [];
    throw err;
  }
};

export const createRoom = (name: string) =>
  http.post<ChatRoom>('/api/chatroom/createChatRoom', { name }).then((r) => r.data);
