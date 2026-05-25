import http from './http';
import type { Message } from '../types/types';

export const getMessages = async (roomId: number, page = 1): Promise<Message[]> => {
  try {
    const res = await http.get<Message[]>(`/api/rooms/${roomId}/messages`, {
      params: { page },
    });
    return res.data.reverse();
  } catch (err: any) {
    if (err.response?.status === 404) return [];
    throw err;
  }
};
